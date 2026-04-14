using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.ChatModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Persistencs.Repositories
{
    public class ChatRepository(EduCoreDbContext context) : IChatRepository
    {
        public async Task SaveMessageAsync(ChatMessage message)
        {
            await context.ChatMessages.AddAsync(message);
        }

        public async Task<List<ChatMessage>> GetConversationHistoryAsync(string userId, int limit)
        {
            var messages = await context.ChatMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();

            messages.Reverse();
            return messages;
        }

        public async Task ClearHistoryAsync(string userId)
        {
            var messages = await context.ChatMessages
                .Where(m => m.UserId == userId)
                .ToListAsync();

            context.ChatMessages.RemoveRange(messages);
        }
    }
}
