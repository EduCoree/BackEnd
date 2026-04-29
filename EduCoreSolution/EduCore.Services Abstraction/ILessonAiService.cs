using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.LessonAi;

namespace EduCore.Services_Abstraction
{
    public interface ILessonAiService
    {
        Task<Result<LessonAiResponseDto>> AskAsync(
            string studentId,
            LessonAiRequestDto dto,
            CancellationToken ct = default);

        Task<Result<LessonAiResponseDto>> SummarizeAsync(
            string studentId,
            LessonAiRequestDto dto,
            CancellationToken ct = default);

        Task<Result<LessonAiResponseDto>> TranslateAsync(
            string studentId,
            LessonAiRequestDto dto,
            CancellationToken ct = default);

        Task<Result<LessonAiResponseDto>> TranscribeAsync(
            string studentId,
            int lessonId,
            CancellationToken ct = default);
    }
}
