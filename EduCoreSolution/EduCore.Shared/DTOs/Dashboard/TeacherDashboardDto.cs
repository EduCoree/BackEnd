using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Dashboard
{

    public record TeacherDashboardDto(
        int TotalCourses,
        int PublishedCourses,
        int TotalEnrolledStudents,
        double AverageCourseRating,
        IEnumerable<UpcomingSessionDto> UpcomingSessions,
        IEnumerable<RecentEnrollmentDto> RecentEnrollments
    );

    public record UpcomingSessionDto(
        int SessionId,
        string LessonTitle,
        string CourseTitle,
        DateTime ScheduledAt,
        string MeetingUrl
    );

    public record RecentEnrollmentDto(
        int EnrollmentId,
        string StudentName,
        string? StudentAvatarUrl,
        string CourseTitle,
        DateTime EnrolledAt
    );
}
