using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Dashboard
{
    public record StudentDashboardDto(
    int EnrolledCourses,
    int CompletedCourses,
    int CertificatesEarned,
    double OverallProgressPercent,
    IEnumerable<UpcomingSessionDto> UpcomingSessions,
    IEnumerable<RecentQuizResultDto> RecentQuizResults
);

    public record RecentQuizResultDto(
        int AttemptId,
        string QuizTitle,
        string CourseTitle,
        decimal? Score,
        bool Passed,
        DateTime? SubmittedAt
    );
}
