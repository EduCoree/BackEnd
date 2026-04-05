using EduCore.Shared.DTOs.Content;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ILessonService
    {
        Task<LessonResponse> CreateLessonAsync(int courseId, string teacherId,
            CreateLessonRequest request, CancellationToken ct = default);

        Task<LessonResponse> UpdateLessonAsync(int courseId, int lessonId, string teacherId,
            UpdateLessonRequest request, CancellationToken ct = default);

        Task DeleteLessonAsync(int courseId, int lessonId, string teacherId,
            CancellationToken ct = default);

        Task<VideoLessonResponse> AddVideoAsync(int courseId, int lessonId, string teacherId,
            AddVideoLessonRequest request, CancellationToken ct = default);

        Task RemoveVideoAsync(int courseId, int lessonId, string teacherId,
            CancellationToken ct = default);

        Task<PdfLessonResponse> AddPdfAsync(int courseId, int lessonId, string teacherId,
            AddPdfLessonRequest request, CancellationToken ct = default);

        Task RemovePdfAsync(int courseId, int lessonId, string teacherId,
            CancellationToken ct = default);

        Task ToggleFreePreviewAsync(int courseId, int lessonId, string teacherId,
            bool isFreePreview, CancellationToken ct = default);
    }
}
