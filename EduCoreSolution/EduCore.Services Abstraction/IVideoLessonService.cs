using EduCore.Shared.DTOs.Content;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IVideoLessonService
    {
        Task<SignedUrlResponse> GetSignedUrlAsync(int lessonId, string studentId,
            CancellationToken ct = default);
    }
}
