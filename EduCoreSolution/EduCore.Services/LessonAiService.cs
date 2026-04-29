using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.LessonAi;
using EduCore.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode.Videos.Streams;

namespace EduCore.Services
{
    public class LessonAiService(
        IUnitOfWork uow,
        IHttpClientFactory httpClientFactory,
        IOptions<GroqSettings> groqOptions,
        ILogger<LessonAiService> logger) : ILessonAiService
    {
        private const int MaxContextLength = 8000;

        // ────────────────────── Ask ──────────────────────

        public async Task<Result<LessonAiResponseDto>> AskAsync(
            string studentId, LessonAiRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Question))
                return Error.Validation("LessonAi.EmptyQuestion", "Question cannot be empty for the ask endpoint.");

            var contextResult = await BuildLessonContextAsync(studentId, dto.LessonId);
            if (contextResult.IsFailure)
                return Result<LessonAiResponseDto>.Fail(contextResult.Errors.ToList());

            var systemPrompt = $"""
                You are EduCore Lesson Assistant, an AI study helper for the EduCore e-learning platform.

                You will be given the context of a specific lesson. Answer the student's question using ONLY the lesson context provided below.
                If the answer is NOT found in the lesson context, say that clearly, then optionally provide a brief general explanation.

                Rules:
                - Be concise, friendly, and academic in tone
                - Respond in the same language the student writes in (Arabic or English)
                - Use bullet points for steps, markdown for code or formulas
                - Keep responses under 300 words unless a detailed explanation is truly needed

                <lesson_context>
                {contextResult.Value}
                </lesson_context>
                """;

            return await CallGroqAsync(systemPrompt, dto.Question, ct);
        }

        // ────────────────────── Summarize ──────────────────────

        public async Task<Result<LessonAiResponseDto>> SummarizeAsync(
            string studentId, LessonAiRequestDto dto, CancellationToken ct = default)
        {
            var contextResult = await BuildLessonContextAsync(studentId, dto.LessonId);
            if (contextResult.IsFailure)
                return Result<LessonAiResponseDto>.Fail(contextResult.Errors.ToList());

            var systemPrompt = $"""
                You are EduCore Lesson Assistant, an AI study helper for the EduCore e-learning platform.

                Summarize the lesson context provided below. Return the following sections:
                1. **Short Summary** — A concise overview of the lesson (2-3 sentences)
                2. **Key Points** — The most important concepts as bullet points
                3. **Important Terms** — Key terminology with brief definitions
                4. **Suggested Revision Questions** — 3-5 questions a student could use to test their understanding

                Rules:
                - Be concise, friendly, and academic in tone
                - Respond in the same language the lesson content is in (Arabic or English)
                - Use markdown formatting

                <lesson_context>
                {contextResult.Value}
                </lesson_context>
                """;

            var userMessage = dto.Question ?? "Please summarize this lesson.";
            return await CallGroqAsync(systemPrompt, userMessage, ct);
        }

        // ────────────────────── Translate ──────────────────────

        public async Task<Result<LessonAiResponseDto>> TranslateAsync(
            string studentId, LessonAiRequestDto dto, CancellationToken ct = default)
        {
            var contextResult = await BuildLessonContextAsync(studentId, dto.LessonId);
            if (contextResult.IsFailure)
                return Result<LessonAiResponseDto>.Fail(contextResult.Errors.ToList());

            var targetLanguage = string.IsNullOrWhiteSpace(dto.TargetLanguage) ? "Arabic" : dto.TargetLanguage;

            var systemPrompt = $"""
                You are EduCore Lesson Assistant, an AI study helper for the EduCore e-learning platform.

                Translate and simplify the lesson content provided below into {targetLanguage}.
                
                Rules:
                - Maintain the original structure and meaning
                - Simplify complex terminology where appropriate
                - Keep academic tone but make it accessible
                - Use markdown formatting
                - Translate section headers and all content

                <lesson_context>
                {contextResult.Value}
                </lesson_context>
                """;

            var userMessage = dto.Question 
                ?? $"Please translate this lesson content into {targetLanguage}.";
            return await CallGroqAsync(systemPrompt, userMessage, ct);
        }

        // ────────────────────── Helpers ──────────────────────

        private async Task<Result<string>> BuildLessonContextAsync(string studentId, int lessonId)
        {
            // 1. Load lesson
            var lesson = await uow.GetRepository<Lesson, int>().GetByIdAsync(lessonId);
            if (lesson is null || lesson.DeletedAt != null)
                return Error.NotFound("LessonAi.LessonNotFound", "Lesson not found.");

            // 2. Navigate to section → course
            var section = await uow.GetRepository<Section, int>().GetByIdAsync(lesson.SectionId);
            if (section is null)
                return Error.NotFound("LessonAi.SectionNotFound", "Lesson section not found.");

            var course = await uow.GetRepository<Course, int>().GetByIdAsync(section.CourseId);
            if (course is null)
                return Error.NotFound("LessonAi.CourseNotFound", "Course not found.");

            // 3. Check enrollment
            var isEnrolled = await uow.EnrollmentRepository.IsEnrolledAsync(studentId, course.Id);
            if (!isEnrolled)
                return Error.Forbidden("LessonAi.NotEnrolled", "You are not enrolled in this course.");

            // 4. Load related content metadata
            var videoRepo = uow.GetRepository<VideoLesson, int>();
            var allVideos = await videoRepo.GetAllAsync();
            var video = allVideos.FirstOrDefault(v => v.LessonId == lessonId);

            var pdfRepo = uow.GetRepository<PdfLesson, int>();
            var allPdfs = await pdfRepo.GetAllAsync();
            var pdf = allPdfs.FirstOrDefault(p => p.LessonId == lessonId);

            // 5. Build context string
            var context = $"""
                Course: {course.Title}
                Course Description: {course.Description ?? "N/A"}
                Section: {section.Title}
                Lesson Title: {lesson.Title}
                Lesson Type: {lesson.Type}
                Duration: {(lesson.DurationSeconds.HasValue ? $"{lesson.DurationSeconds}s" : "N/A")}
                """;

            if (video != null)
            {
                context += $"""

                    Video Provider: {video.VideoProvider ?? "N/A"}
                    Video URL: {video.VideoUrl}
                    """;

                // Include transcript if available
                if (!string.IsNullOrWhiteSpace(video.Transcript))
                {
                    context += $"""

                        Video Transcript:
                        {video.Transcript}
                        """;
                }
            }

            if (pdf != null)
            {
                context += $"""

                    PDF File: {pdf.FileUrl}
                    PDF Size: {(pdf.FileSizeKb.HasValue ? $"{pdf.FileSizeKb}KB" : "N/A")}
                    """;
            }

            // 6. Truncate to max length
            if (context.Length > MaxContextLength)
            {
                context = context[..MaxContextLength] + "\n... [truncated]";
                logger.LogWarning("Lesson context for LessonId={LessonId} was truncated to {MaxLength} chars.",
                    lessonId, MaxContextLength);
            }

            return Result<string>.Ok(context);
        }

        // ────────────────────── Transcribe (Whisper) ──────────────────────

        public async Task<Result<LessonAiResponseDto>> TranscribeAsync(
            string studentId, int lessonId, CancellationToken ct = default)
        {
            // 1. Load lesson + enrollment check
            var lesson = await uow.GetRepository<Lesson, int>().GetByIdAsync(lessonId);
            if (lesson is null || lesson.DeletedAt != null)
                return Error.NotFound("LessonAi.LessonNotFound", "Lesson not found.");

            var section = await uow.GetRepository<Section, int>().GetByIdAsync(lesson.SectionId);
            if (section is null)
                return Error.NotFound("LessonAi.SectionNotFound", "Section not found.");

            var course = await uow.GetRepository<Course, int>().GetByIdAsync(section.CourseId);
            if (course is null)
                return Error.NotFound("LessonAi.CourseNotFound", "Course not found.");

            var isEnrolled = await uow.EnrollmentRepository.IsEnrolledAsync(studentId, course.Id);
            if (!isEnrolled)
                return Error.Forbidden("LessonAi.NotEnrolled", "You are not enrolled in this course.");

            // 2. Get video
            var videoRepo = uow.GetRepository<VideoLesson, int>();
            var allVideos = await videoRepo.GetAllAsync();
            var video = allVideos.FirstOrDefault(v => v.LessonId == lessonId);

            if (video is null)
                return Error.NotFound("LessonAi.NoVideo", "This lesson has no video to transcribe.");

            // 3. If already transcribed, return it
            if (!string.IsNullOrWhiteSpace(video.Transcript))
            {
                return Result<LessonAiResponseDto>.Ok(new LessonAiResponseDto
                {
                    Answer = video.Transcript,
                    CreatedAt = video.TranscribedAt ?? DateTime.UtcNow
                });
            }

            // 4. Detect provider and transcribe accordingly
            try
            {
                string transcript;
                var videoUrl = video.VideoUrl;

                if (IsYouTubeUrl(videoUrl))
                {
                    // ── YouTube: try captions first, fall back to audio + Whisper ──
                    transcript = await FetchYouTubeCaptionsAsync(videoUrl, ct);

                    if (string.IsNullOrWhiteSpace(transcript))
                    {
                        logger.LogInformation("No captions found, downloading YouTube audio for Whisper transcription...");
                        var audioBytes = await DownloadYouTubeAudioAsync(videoUrl, ct);
                        transcript = await WhisperTranscribeFromBytesAsync(audioBytes, ct);
                    }
                }
                else
                {
                    // ── Self-hosted: download audio → Groq Whisper ──
                    transcript = await WhisperTranscribeAsync(videoUrl, ct);
                }

                if (string.IsNullOrWhiteSpace(transcript))
                {
                    return Error.Failure("LessonAi.NoTranscript",
                        "Could not extract transcript from this video. No captions or audio available.");
                }

                // 5. Save transcript to DB
                video.Transcript = transcript;
                video.TranscribedAt = DateTime.UtcNow;
                await uow.SaveChangesAsync();

                logger.LogInformation("Transcribed LessonId={LessonId}, length={Length} chars.",
                    lessonId, transcript.Length);

                return Result<LessonAiResponseDto>.Ok(new LessonAiResponseDto
                {
                    Answer = transcript,
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to transcribe LessonId={LessonId}", lessonId);
                return Error.Failure("LessonAi.TranscribeError",
                    "An error occurred during transcription. Please try again.");
            }
        }

        private async Task<Result<LessonAiResponseDto>> CallGroqAsync(
            string systemPrompt, string userMessage, CancellationToken ct)
        {
            var settings = groqOptions.Value;

            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            };

            var requestBody = new
            {
                model = settings.Model,
                messages,
                max_tokens = 2048
            };

            try
            {
                var client = httpClientFactory.CreateClient("GroqClient");

                var response = await client.PostAsJsonAsync("chat/completions", requestBody, ct);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(ct);
                    logger.LogError("Groq API error: {StatusCode} — {Body}", response.StatusCode, errorBody);
                    return Error.Failure("LessonAi.ApiFailed",
                        $"AI service returned {response.StatusCode}. Please try again later.");
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
                var aiReply = json
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                return Result<LessonAiResponseDto>.Ok(new LessonAiResponseDto
                {
                    Answer = aiReply,
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error calling Groq API.");
                return Error.Failure("LessonAi.UnexpectedError",
                    "An unexpected error occurred while processing your request.");
            }
        }

        // ────────────────────── YouTube Helpers ──────────────────────

        private static bool IsYouTubeUrl(string url)
        {
            return url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase)
                || url.Contains("youtu.be", StringComparison.OrdinalIgnoreCase);
        }

        private static string? ExtractYouTubeVideoId(string url)
        {
            // Handle: https://youtu.be/VIDEO_ID
            if (url.Contains("youtu.be/"))
            {
                var id = url.Split("youtu.be/")[1].Split('?')[0].Split('&')[0];
                return string.IsNullOrEmpty(id) ? null : id;
            }
            // Handle: https://www.youtube.com/watch?v=VIDEO_ID
            if (url.Contains("v="))
            {
                var id = url.Split("v=")[1].Split('&')[0];
                return string.IsNullOrEmpty(id) ? null : id;
            }
            return null;
        }

        private async Task<string> FetchYouTubeCaptionsAsync(string videoUrl, CancellationToken ct)
        {
            var videoId = ExtractYouTubeVideoId(videoUrl);
            if (string.IsNullOrEmpty(videoId))
                throw new InvalidOperationException("Could not extract YouTube video ID.");

            var youtube = new YoutubeClient();

            // Get available caption tracks
            var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId, ct);

            // Try Arabic first (most videos on this platform), then English, then any
            var track = trackManifest.Tracks
                            .FirstOrDefault(t => t.Language.Code.StartsWith("ar"))
                        ?? trackManifest.Tracks
                            .FirstOrDefault(t => t.Language.Code.StartsWith("en"))
                        ?? trackManifest.Tracks.FirstOrDefault();

            if (track is null)
            {
                logger.LogWarning("No caption tracks found for YouTube video {VideoId}", videoId);
                return string.Empty;
            }

            logger.LogInformation("Found caption track: {Lang} for video {VideoId}",
                track.Language.Code, videoId);

            // Download the caption track
            var captionTrack = await youtube.Videos.ClosedCaptions.GetAsync(track, ct);

            // Join all caption text into a single string
            var transcript = string.Join(" ", captionTrack.Captions.Select(c => c.Text));

            logger.LogInformation("Fetched captions for YouTube video {VideoId}, length={Length}",
                videoId, transcript.Length);

            return transcript;
        }

        // ────────────────────── YouTube Audio Download ──────────────────────

        private async Task<byte[]> DownloadYouTubeAudioAsync(string videoUrl, CancellationToken ct)
        {
            var videoId = ExtractYouTubeVideoId(videoUrl);
            if (string.IsNullOrEmpty(videoId))
                throw new InvalidOperationException("Could not extract YouTube video ID.");

            var youtube = new YoutubeClient();

            // Get available streams
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId, ct);

            // Get the best audio-only stream (smallest for speed)
            var audioStream = streamManifest.GetAudioOnlyStreams()
                .OrderBy(s => s.Size.Bytes)
                .FirstOrDefault();

            if (audioStream is null)
                throw new InvalidOperationException("No audio stream available for this YouTube video.");

            logger.LogInformation("Downloading YouTube audio: {Bitrate}kbps, {Size}MB",
                audioStream.Bitrate.KiloBitsPerSecond,
                audioStream.Size.MegaBytes.ToString("F1"));

            // Download to memory
            using var memoryStream = new MemoryStream();
            await youtube.Videos.Streams.CopyToAsync(audioStream, memoryStream, cancellationToken: ct);
            var audioBytes = memoryStream.ToArray();

            if (audioBytes.Length > 25 * 1024 * 1024)
                throw new InvalidOperationException("Audio file exceeds 25MB Whisper limit.");

            return audioBytes;
        }

        // ────────────────────── Whisper (Self-hosted videos) ──────────────────────

        private async Task<string> WhisperTranscribeAsync(string videoUrl, CancellationToken ct)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(2);
            var audioBytes = await httpClient.GetByteArrayAsync(videoUrl, ct);

            return await WhisperTranscribeFromBytesAsync(audioBytes, ct);
        }

        private async Task<string> WhisperTranscribeFromBytesAsync(byte[] audioBytes, CancellationToken ct)
        {
            if (audioBytes.Length > 25 * 1024 * 1024)
                throw new InvalidOperationException("Audio file exceeds 25MB limit for transcription.");

            var groqClient = httpClientFactory.CreateClient("GroqClient");

            using var formContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(audioBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");
            formContent.Add(fileContent, "file", "audio.mp3");
            formContent.Add(new StringContent("whisper-large-v3-turbo"), "model");
            formContent.Add(new StringContent("text"), "response_format");

            var response = await groqClient.PostAsync("audio/transcriptions", formContent, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("Whisper API error: {Status} — {Body}", response.StatusCode, errorBody);
                throw new InvalidOperationException($"Whisper transcription failed: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync(ct);
        }
    }
}
