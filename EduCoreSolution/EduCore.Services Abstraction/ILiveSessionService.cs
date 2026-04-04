using EduCore.Shared.DTOs.Content;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ILiveSessionService
    {
        // ── Teacher ──────────────────────────────────────
        Task<List<LiveSessionResponse>> GetSessionsAsync(int courseId, string teacherId,
            CancellationToken ct = default);

        Task<LiveSessionResponse> CreateSessionAsync(int courseId, string teacherId,
            CreateLiveSessionRequest request, CancellationToken ct = default);

        Task<LiveSessionResponse> UpdateSessionAsync(int courseId, int sessionId, string teacherId,
            UpdateLiveSessionRequest request, CancellationToken ct = default);

        Task DeleteSessionAsync(int courseId, int sessionId, string teacherId,
            CancellationToken ct = default);

        Task UpdateRecordingAsync(int courseId, int sessionId, string teacherId,
            string recordingUrl, CancellationToken ct = default);

        // ── Student ──────────────────────────────────────
        Task<List<LiveSessionResponse>> GetUpcomingSessionsAsync(string studentId,
            CancellationToken ct = default);

        Task<string> GetJoinUrlAsync(int sessionId, string studentId,
            CancellationToken ct = default);
    }
}
