using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ChatModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Chat;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Text.Json;

namespace EduCore.Services
{
    public class ChatService(
        IUnitOfWork uow,
        IHttpClientFactory httpClientFactory,
        UserManager<User> userManager) : IChatService
    {
        private const string SystemPrompt = """
            You are EduCore Assistant, an AI academic support chatbot for the EduCore e-learning platform.

            Help students understand course material, navigate the platform, and prepare for quizzes. Help teachers with course management advice. Help admins with moderation questions.

            Rules:
            - Be concise, friendly, and academic in tone
            - Never reveal API routes, database details, or system internals
            - If asked something unrelated to education or EduCore, politely redirect
            - Respond in the same language the user writes in (Arabic or English)
            - Use bullet points for steps, markdown for code or formulas
            - Keep responses under 200 words unless a detailed explanation is truly needed

            You will receive a context block with user info. Use it to personalize responses.
            """;

        public async Task<Result<ChatResponseDto>> SendMessageAsync(
            string userId,
            string userRole,
            ChatRequestDto dto,
            CancellationToken ct = default)
        {
            var history = await uow.ChatRepository.GetConversationHistoryAsync(userId, 10);

            string? courseTitle = null;
            if (dto.CourseId.HasValue)
            {
                var course = await uow.CourseRepository.GetByIdAsync(dto.CourseId.Value);
                courseTitle = course?.Title;
            }

            var user = await userManager.FindByIdAsync(userId);
            var userName = user?.Name ?? "Unknown";

            var contextBlock = $"""
                <context>
                  userRole: {userRole}
                  userName: {userName}
                  currentCourse: {courseTitle ?? "null"}
                </context>
                """;

            var messages = new List<object>();
            foreach (var msg in history)
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }
            messages.Add(new { role = "user", content = contextBlock + "\n\n" + dto.Message });

            var requestBody = new
            {
                model = "claude-sonnet-4-20250514",
                max_tokens = 1024,
                system = SystemPrompt,
                messages
            };

            var client = httpClientFactory.CreateClient("AnthropicClient");

            var response = await client.PostAsJsonAsync(
                "https://api.anthropic.com/v1/messages",
                requestBody,
                ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                return Error.Failure("Chat.ApiFailed", $"Anthropic API returned {response.StatusCode}: {errorBody}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            var assistantReply = json
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            var now = DateTime.UtcNow;

            await uow.ChatRepository.SaveMessageAsync(new ChatMessage
            {
                UserId = userId,
                Role = "user",
                Content = dto.Message,
                CreatedAt = now
            });

            await uow.ChatRepository.SaveMessageAsync(new ChatMessage
            {
                UserId = userId,
                Role = "assistant",
                Content = assistantReply,
                CreatedAt = now
            });

            await uow.SaveChangesAsync();

            return Result<ChatResponseDto>.Ok(new ChatResponseDto
            {
                Reply = assistantReply,
                CreatedAt = now
            });
        }

        public async Task<Result> ClearHistoryAsync(string userId, CancellationToken ct = default)
        {
            await uow.ChatRepository.ClearHistoryAsync(userId);
            await uow.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
