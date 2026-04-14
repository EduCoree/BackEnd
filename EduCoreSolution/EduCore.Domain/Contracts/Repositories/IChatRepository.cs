using EduCore.Domain.Entities.ChatModel;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IChatRepository
    {
        Task SaveMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetConversationHistoryAsync(string userId, int limit);
        Task ClearHistoryAsync(string userId);
    }
}
