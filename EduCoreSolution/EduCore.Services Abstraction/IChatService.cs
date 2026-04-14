using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Chat;

namespace EduCore.Services_Abstraction
{
    public interface IChatService
    {
        Task<Result<ChatResponseDto>> SendMessageAsync(
            string userId,
            string userRole,
            ChatRequestDto dto,
            CancellationToken ct = default);

        Task<Result> ClearHistoryAsync(string userId, CancellationToken ct = default);
    }
}
