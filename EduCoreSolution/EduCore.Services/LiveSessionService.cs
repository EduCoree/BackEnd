using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Content;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class LiveSessionService : ILiveSessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;

        public LiveSessionService(IUnitOfWork uow,INotificationService notificationService)
        {
            _uow = uow;
            _notificationService = notificationService;
        }

        // ── Teacher ──────────────────────────────────────

        public async Task<List<LiveSessionResponse>> GetSessionsAsync(
            int courseId, string teacherId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var sessionRepo = _uow.GetRepository<LiveSession, int>();
            var allSessions = await sessionRepo.GetAllAsync();
            var courseSessions = allSessions.Where(s => s.CourseId == courseId).ToList();

            return courseSessions.Select(MapToResponse).ToList();
        }

        public async Task<LiveSessionResponse> CreateSessionAsync(
            int courseId, string teacherId, CreateLiveSessionRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            if (!Enum.TryParse<LiveProvider>(request.Provider, true, out var providerEnum))
                throw new BadRequestException("Invalid provider.");

            // Auto-generate room name for Jitsi; other providers use the teacher-provided URL
            var meetingUrl = providerEnum == LiveProvider.Jitsi
                ? $"educore-{courseId}-{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                : request.MeetingUrl ?? throw new BadRequestException("Meeting URL is required for this provider.");

            var session = new LiveSession
            {
                CourseId = courseId,
                Provider = providerEnum,
                MeetingUrl = meetingUrl,
                ScheduledAt = request.ScheduledAt,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.GetRepository<LiveSession, int>().AddAsync(session);
            await _uow.SaveChangesAsync();
            if (session.ScheduledAt >= DateTime.UtcNow)
            {

                var ActiveStudents = await _uow.EnrollmentRepository.GetActiveStudentIdsByCourseAsync(courseId);
                foreach (var studentid in ActiveStudents)
                {
                    await _notificationService.SendNotificationAsync(studentid, "New Live Session", $"A new session '{session.Title}' has been scheduled", NotificationType.LiveSession, session.Id);
                }
            }

            return MapToResponse(session);
        }

        public async Task<LiveSessionResponse> UpdateSessionAsync(
            int courseId, int sessionId, string teacherId, UpdateLiveSessionRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var session = await GetSessionInCourse(courseId, sessionId);

            if (request.Provider != null)
            {
                if (!Enum.TryParse<LiveProvider>(request.Provider, true, out var providerEnum))
                    throw new BadRequestException("Invalid provider.");
                session.Provider = providerEnum;
            }

            // Jitsi room names are auto-generated and immutable
            if (request.MeetingUrl != null)
            {
                if (session.Provider == LiveProvider.Jitsi)
                    throw new BadRequestException("Cannot change the meeting URL for Jitsi sessions. Room names are auto-generated.");
                session.MeetingUrl = request.MeetingUrl;
            }

            if (request.ScheduledAt.HasValue) session.ScheduledAt = request.ScheduledAt.Value;
            if (request.Title != null) session.Title = request.Title;
            if (request.Description != null) session.Description = request.Description;

            _uow.GetRepository<LiveSession, int>().Update(session);
            await _uow.SaveChangesAsync();

            return MapToResponse(session);
        }

        public async Task DeleteSessionAsync(
            int courseId, int sessionId, string teacherId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var session = await GetSessionInCourse(courseId, sessionId);

            // Fetch enrolled students to notify
            var enrollmentRepo = _uow.GetRepository<Enrollment, int>();
            var allEnrollments = await enrollmentRepo.GetAllAsync();
            var activeStudents = allEnrollments
                .Where(e => e.CourseId == courseId && e.Status == EnrollmentStatus.Active)
                .Select(e => e.StudentId)
                .ToList();

            //var notifyRepo = _uow.GetRepository<Notification, int>();
            //var courseName = await _uow.CourseRepository.GetByIdAsync(courseId);

            //foreach (var studentId in activeStudents)
            //{
            //    await notifyRepo.AddAsync(new Notification
            //    {
            //        UserId = studentId,
            //        Type = "session_cancelled",
            //        Title = "Live Session Cancelled",
            //        Message = $"The live session '{session.Title ?? "Scheduled Session"}' for course '{courseName?.Title}' has been cancelled.",
            //        CreatedAt = DateTime.UtcNow
            //    });
            //}
            
            _uow.GetRepository<LiveSession, int>().Remove(session);
            await _uow.SaveChangesAsync();
            if (session.ScheduledAt >= DateTime.UtcNow)
            {
                var ActiveStudents = await _uow.EnrollmentRepository.GetActiveStudentIdsByCourseAsync(courseId);
                foreach (var studentid in ActiveStudents)
                {
                    await _notificationService.SendNotificationAsync(studentid, "Live Session Cancelled", $"The session '{session.Title}' has been Cancelled", NotificationType.SessionCancelled, session.Id);
                }
            }
        }

        public async Task UpdateRecordingAsync(
            int courseId, int sessionId, string teacherId, string recordingUrl, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var session = await GetSessionInCourse(courseId, sessionId);
            
            if (session.ScheduledAt > DateTime.UtcNow)
                throw new BadRequestException("Cannot add recording to a future session.");

            session.RecordingUrl = recordingUrl;
            _uow.GetRepository<LiveSession, int>().Update(session);
            await _uow.SaveChangesAsync();
        }

        // ── Student ──────────────────────────────────────

        public async Task<List<LiveSessionResponse>> GetUpcomingSessionsAsync(
            string studentId, CancellationToken ct)
        {
            var enrollmentRepo = _uow.GetRepository<Enrollment, int>();
            var allEnrollments = await enrollmentRepo.GetAllAsync();
            var enrolledCourseIds = allEnrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
                .Select(e => e.CourseId)
                .Distinct()
                .ToList();

            var sessionRepo = _uow.GetRepository<LiveSession, int>();
            var allSessions = await sessionRepo.GetAllAsync();
            
            var upcomingSessions = allSessions
                .Where(s => enrolledCourseIds.Contains(s.CourseId) && s.ScheduledAt >= DateTime.UtcNow.AddHours(-2)) 
                .OrderBy(s => s.ScheduledAt)
                .ToList();

            // Fetch course and teacher info
            var courses = await _uow.CourseRepository.GetCoursesWithTeacherAsync(upcomingSessions.Select(s => s.CourseId).Distinct());
            var courseDict = courses.ToDictionary(c => c.Id);

            return upcomingSessions.Select(s => 
            {
                var response = MapToResponse(s);
                if (courseDict.TryGetValue(s.CourseId, out var course))
                {
                    response.CourseName = course.Title;
                    response.TeacherName = course.Teacher?.Name;
                }
                return response;
            }).ToList();
        }

        public async Task<JoinSessionResponse> GetJoinUrlAsync(
            int sessionId, string studentId, CancellationToken ct)
        {
            var session = await _uow.GetRepository<LiveSession, int>().GetByIdAsync(sessionId);
            if (session is null)
                throw new NotFoundException("Session not found.");

            // Verify enrollment
            var enrollmentRepo = _uow.GetRepository<Enrollment, int>();
            var allEnrollments = await enrollmentRepo.GetAllAsync();
            var isActiveEnrolled = allEnrollments.Any(e => 
                e.CourseId == session.CourseId && 
                e.StudentId == studentId && 
                e.Status == EnrollmentStatus.Active);

            if (!isActiveEnrolled)
                throw new ForbiddenException("You must be enrolled in the course to join this session.");

            // 15 min rule — compare in UTC to avoid server timezone offset bugs
            if (DateTime.UtcNow < session.ScheduledAt.AddMinutes(-15))
                throw new ForbiddenException("Session has not started yet. You can join up to 15 minutes before the start time.");

            return new JoinSessionResponse
            {
                RoomName = session.MeetingUrl,
                Provider = session.Provider.ToString(),
                Title = session.Title
            };
        }

        // ── Helpers ──────────────────────────────────────

        private async Task EnsureCourseOwnership(int courseId, string teacherId)
        {
            var ownerTeacherId = await _uow.CourseRepository.GetCourseTeacherIdAsync(courseId);
            if (ownerTeacherId is null)
                throw new NotFoundException("Course not found.");
            if (ownerTeacherId != teacherId)
                throw new UnauthorizedException("You are not the owner of this course.");
        }

        private async Task<LiveSession> GetSessionInCourse(int courseId, int sessionId)
        {
            var session = await _uow.GetRepository<LiveSession, int>().GetByIdAsync(sessionId);
            if (session is null || session.CourseId != courseId)
                throw new NotFoundException("Session not found in this course.");
            return session;
        }

        private static LiveSessionResponse MapToResponse(LiveSession session)
        {
            return new LiveSessionResponse
            {
                Id = session.Id,
                CourseId = session.CourseId,
                Provider = session.Provider.ToString(),
                MeetingUrl = session.MeetingUrl,
                ScheduledAt = session.ScheduledAt,
                RecordingUrl = session.RecordingUrl,
                Title = session.Title,
                Description = session.Description,
                CreatedAt = session.CreatedAt
            };
        }
    }
}
