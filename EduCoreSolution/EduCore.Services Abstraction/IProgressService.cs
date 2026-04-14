using EduCore.Shared.DTOs.Progress;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IProgressService
    {
        // ── Student ──────────────────────────────────────

        Task RecordWatchAsync(string studentId, int lessonId,
            int positionSecs, CancellationToken ct = default);

        Task<LessonProgressResponse> CompleteLessonAsync(string studentId,
            int lessonId, CancellationToken ct = default);

        Task<CourseProgressResponse> GetCourseProgressAsync(string studentId,
            int courseId, CancellationToken ct = default);

        Task<ResumeLessonResponse> GetResumeLessonAsync(string studentId,
            int courseId, CancellationToken ct = default);

        // ── Teacher ──────────────────────────────────────

        Task<List<StudentProgressSummaryResponse>> GetStudentsProgressAsync(
            string teacherId, int courseId, CancellationToken ct = default);

        Task<StudentLessonDetailResponse> GetStudentDetailAsync(string teacherId,
            int courseId, string studentId, CancellationToken ct = default);
    }
}
