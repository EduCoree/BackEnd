using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Progress;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;

        public ProgressService(IUnitOfWork uow, UserManager<User> userManager,INotificationService notificationService)
        {
            _uow = uow;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // ── Student ──────────────────────────────────────

        public async Task RecordWatchAsync(string studentId, int lessonId,
            int positionSecs, CancellationToken ct)
        {
            var lesson = await _uow.GetRepository<Lesson, int>().GetByIdAsync(lessonId);
            if (lesson is null || lesson.DeletedAt != null)
                throw new NotFoundException("Lesson not found.");

            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(lesson.SectionId);
            var courseId = section!.CourseId;

            EnsureActiveEnrollment(await GetEnrollmentsAsync(), studentId, courseId);

            var progressRepo = _uow.GetRepository<LessonProgress, int>();
            var allProgress = await progressRepo.GetAllAsync();
            var match = allProgress.FirstOrDefault(p =>
                p.StudentId == studentId && p.LessonId == lessonId);

            if (match != null)
            {
                match.LastPositionSecs = positionSecs;
                progressRepo.Update(match);
            }
            else
            {
                await progressRepo.AddAsync(new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    LastPositionSecs = positionSecs,
                    IsCompleted = false
                });
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<LessonProgressResponse> CompleteLessonAsync(string studentId,
            int lessonId, CancellationToken ct)
        {
            var lesson = await _uow.GetRepository<Lesson, int>().GetByIdAsync(lessonId);
            if (lesson is null || lesson.DeletedAt != null)
                throw new NotFoundException("Lesson not found.");

            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(lesson.SectionId);
            var courseId = section!.CourseId;

            EnsureActiveEnrollment(await GetEnrollmentsAsync(), studentId, courseId);

            var progressRepo = _uow.GetRepository<LessonProgress, int>();
            var allProgress = await progressRepo.GetAllAsync();
            var match = allProgress.FirstOrDefault(p =>
                p.StudentId == studentId && p.LessonId == lessonId);

            if (match != null)
            {
                match.IsCompleted = true;
                match.CompletedAt = DateTime.UtcNow;
                progressRepo.Update(match);
            }
            else
            {
                match = new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    LastPositionSecs = 0
                };
                await progressRepo.AddAsync(match);
            }

            await _uow.SaveChangesAsync();

            // ── Certificate auto-issue ──────────────────
            var courseLessons = await GetCourseLessonsAsync(courseId);
            var updatedProgress = await progressRepo.GetAllAsync();
            var completedIds = updatedProgress
                .Where(p => p.StudentId == studentId && p.IsCompleted)
                .Select(p => p.LessonId)
                .ToList();

            if (courseLessons.Count > 0 && courseLessons.All(l => completedIds.Contains(l.Id)))
            {
                var certRepo = _uow.GetRepository<Certificate, int>();
                var allCerts = await certRepo.GetAllAsync();
                var exists = allCerts.Any(c =>
                    c.StudentId == studentId && c.CourseId == courseId);

                if (!exists)
                {
                    await certRepo.AddAsync(new Certificate
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        IssuedAt = DateTime.UtcNow,
                        CertificateUrl = $"/certificates/{Guid.NewGuid():N}"
                    });
                    await _uow.SaveChangesAsync();

                    await _notificationService.SendNotificationAsync(
                    userId: studentId,
                    title: "Certificate Earned!",
                    message: "Congratulations! You've completed the course and earned your certificate.",
                    notificationType: NotificationType.Certificate,
                    entityId: courseId
    );
                }
            }

            return new LessonProgressResponse
            {
                LessonId = match.LessonId,
                IsCompleted = match.IsCompleted,
                LastPositionSecs = match.LastPositionSecs ?? 0,
                CompletedAt = match.CompletedAt
            };
        }

        public async Task<CourseProgressResponse> GetCourseProgressAsync(string studentId,
            int courseId, CancellationToken ct)
        {
            EnsureActiveEnrollment(await GetEnrollmentsAsync(), studentId, courseId);

            var courseLessons = await GetCourseLessonsAsync(courseId);
            var total = courseLessons.Count;
            var courseLessonIds = courseLessons.Select(l => l.Id).ToList();

            var allProgress = await _uow.GetRepository<LessonProgress, int>().GetAllAsync();
            var completed = allProgress.Count(p =>
                p.StudentId == studentId &&
                p.IsCompleted &&
                courseLessonIds.Contains(p.LessonId));

            var percent = total == 0 ? 0 : Math.Round((completed / (double)total) * 100, 2);

            var allCerts = await _uow.GetRepository<Certificate, int>().GetAllAsync();
            var certIssued = allCerts.Any(c =>
                c.StudentId == studentId && c.CourseId == courseId);

            return new CourseProgressResponse
            {
                CourseId = courseId,
                TotalLessons = total,
                CompletedLessons = completed,
                PercentComplete = percent,
                CertificateIssued = certIssued
            };
        }

        public async Task<ResumeLessonResponse> GetResumeLessonAsync(string studentId,
            int courseId, CancellationToken ct)
        {
            EnsureActiveEnrollment(await GetEnrollmentsAsync(), studentId, courseId);

            var courseLessons = await GetCourseLessonsAsync(courseId);
            var orderedLessons = courseLessons.OrderBy(l => l.SortOrder).ToList();
            var courseLessonIds = orderedLessons.Select(l => l.Id).ToList();

            var allProgress = await _uow.GetRepository<LessonProgress, int>().GetAllAsync();
            var studentProgress = allProgress
                .Where(p => p.StudentId == studentId && courseLessonIds.Contains(p.LessonId))
                .ToList();

            // First priority: in-progress lesson with a saved position
            var inProgress = studentProgress
                .Where(p => (p.LastPositionSecs ?? 0) > 0 && !p.IsCompleted)
                .OrderByDescending(p => p.LastPositionSecs)
                .FirstOrDefault();

            if (inProgress != null)
            {
                var lpLesson = orderedLessons.First(l => l.Id == inProgress.LessonId);
                return new ResumeLessonResponse
                {
                    LessonId = lpLesson.Id,
                    LessonTitle = lpLesson.Title,
                    LastPositionSecs = inProgress.LastPositionSecs ?? 0
                };
            }

            // Second priority: first lesson with no progress row at all
            var progressLessonIds = studentProgress.Select(p => p.LessonId).ToHashSet();
            var firstUntouched = orderedLessons
                .FirstOrDefault(l => !progressLessonIds.Contains(l.Id));

            if (firstUntouched != null)
            {
                return new ResumeLessonResponse
                {
                    LessonId = firstUntouched.Id,
                    LessonTitle = firstUntouched.Title,
                    LastPositionSecs = 0
                };
            }

            throw new NotFoundException("No resumable lesson found.");
        }

        // ── Teacher ──────────────────────────────────────

        public async Task<List<StudentProgressSummaryResponse>> GetStudentsProgressAsync(
            string teacherId, int courseId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var courseLessons = await GetCourseLessonsAsync(courseId);
            var total = courseLessons.Count;
            var courseLessonIds = courseLessons.Select(l => l.Id).ToList();

            var allEnrollments = await GetEnrollmentsAsync();
            var activeStudents = allEnrollments
                .Where(e => e.CourseId == courseId && e.Status == EnrollmentStatus.Active)
                .ToList();

            var allProgress = await _uow.GetRepository<LessonProgress, int>().GetAllAsync();

            var result = new List<StudentProgressSummaryResponse>();
            foreach (var enrollment in activeStudents)
            {
                var completed = allProgress.Count(p =>
                    p.StudentId == enrollment.StudentId &&
                    p.IsCompleted &&
                    courseLessonIds.Contains(p.LessonId));

                var percent = total == 0 ? 0 : Math.Round((completed / (double)total) * 100, 2);

                var student = await _userManager.FindByIdAsync(enrollment.StudentId);
                var displayName = !string.IsNullOrWhiteSpace(student?.Name)
                    ? student.Name
                    : !string.IsNullOrWhiteSpace(student?.UserName)
                        ? student.UserName
                        : student?.Email ?? enrollment.StudentId;
                result.Add(new StudentProgressSummaryResponse
                {
                    StudentId = enrollment.StudentId,
                    StudentName = displayName,
                    Email = student?.Email ?? string.Empty,
                    CompletedLessons = completed,
                    TotalLessons = total,
                    PercentComplete = percent
                });
            }

            return result;
        }

        public async Task<StudentLessonDetailResponse> GetStudentDetailAsync(string teacherId,
            int courseId, string studentId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var courseLessons = await GetCourseLessonsAsync(courseId);
            var orderedLessons = courseLessons.OrderBy(l => l.SortOrder).ToList();
            var courseLessonIds = orderedLessons.Select(l => l.Id).ToList();

            var allProgress = await _uow.GetRepository<LessonProgress, int>().GetAllAsync();
            var studentProgress = allProgress
                .Where(p => p.StudentId == studentId && courseLessonIds.Contains(p.LessonId))
                .ToList();

            var lessons = orderedLessons.Select(l =>
            {
                var progress = studentProgress.FirstOrDefault(p => p.LessonId == l.Id);
                return new LessonDetailItem
                {
                    LessonId = l.Id,
                    Title = l.Title,
                    IsCompleted = progress?.IsCompleted ?? false,
                    CompletedAt = progress?.CompletedAt,
                    LastPositionSecs = progress?.LastPositionSecs ?? 0
                };
            }).ToList();

            var student = await _userManager.FindByIdAsync(studentId);
            var displayName = !string.IsNullOrWhiteSpace(student?.Name)
                ? student.Name
                : !string.IsNullOrWhiteSpace(student?.UserName)
                    ? student.UserName
                    : student?.Email ?? studentId;
            return new StudentLessonDetailResponse
            {
                StudentId = studentId,
                StudentName = displayName,
                Lessons = lessons
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

        private async Task<IEnumerable<Enrollment>> GetEnrollmentsAsync()
        {
            return await _uow.GetRepository<Enrollment, int>().GetAllAsync();
        }

        private void EnsureActiveEnrollment(IEnumerable<Enrollment> enrollments,
            string studentId, int courseId)
        {
            var isActive = enrollments.Any(e =>
                e.CourseId == courseId &&
                e.StudentId == studentId &&
                e.Status == EnrollmentStatus.Active);

            if (!isActive)
                throw new ForbiddenException("You must be actively enrolled.");
        }

        private async Task<List<Lesson>> GetCourseLessonsAsync(int courseId)
        {
            var allSections = await _uow.GetRepository<Section, int>().GetAllAsync();
            var courseSectionIds = allSections
                .Where(s => s.CourseId == courseId)
                .Select(s => s.Id)
                .ToList();

            var allLessons = await _uow.GetRepository<Lesson, int>().GetAllAsync();
            return allLessons
                .Where(l => courseSectionIds.Contains(l.SectionId) && l.DeletedAt == null)
                .ToList();
        }
    }
}
