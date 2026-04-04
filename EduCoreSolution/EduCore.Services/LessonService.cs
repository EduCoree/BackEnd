using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Content;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EduCore.Services
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _uow;

        public LessonService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ────────────────────── Lesson CRUD ──────────────────────

        public async Task<LessonResponse> CreateLessonAsync(
            int courseId, string teacherId, CreateLessonRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            // Verify section belongs to course
            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(request.SectionId);
            if (section is null || section.CourseId != courseId)
                throw new NotFoundException("Section not found in this course.");

            var lesson = new Lesson
            {
                SectionId = request.SectionId,
                Title = request.Title,
                DurationSeconds = request.DurationSeconds,
                SortOrder = request.SortOrder ?? 0,
                Type = LessonType.None,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.GetRepository<Lesson, int>().AddAsync(lesson);
            await _uow.SaveChangesAsync();

            return MapToResponse(lesson);
        }

        public async Task<LessonResponse> UpdateLessonAsync(
            int courseId, int lessonId, string teacherId, UpdateLessonRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var lesson = await GetLessonInCourse(courseId, lessonId);

            if (request.Title != null) lesson.Title = request.Title;
            if (request.DurationSeconds.HasValue) lesson.DurationSeconds = request.DurationSeconds;
            if (request.SortOrder.HasValue) lesson.SortOrder = request.SortOrder.Value;

            _uow.GetRepository<Lesson, int>().Update(lesson);
            await _uow.SaveChangesAsync();

            return MapToResponse(lesson);
        }

        public async Task DeleteLessonAsync(
            int courseId, int lessonId, string teacherId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);

            var lesson = await GetLessonInCourse(courseId, lessonId);

            // Soft delete — set DeletedAt timestamp
            lesson.DeletedAt = DateTime.UtcNow;
            _uow.GetRepository<Lesson, int>().Update(lesson);

            // Cascade soft-delete: remove associated video_lessons row
            var videoRepo = _uow.GetRepository<VideoLesson, int>();
            var allVideos = await videoRepo.GetAllAsync();
            var video = allVideos.FirstOrDefault(v => v.LessonId == lessonId);
            if (video != null)
                videoRepo.Remove(video);

            // Cascade soft-delete: remove associated pdf_lessons row
            var pdfRepo = _uow.GetRepository<PdfLesson, int>();
            var allPdfs = await pdfRepo.GetAllAsync();
            var pdf = allPdfs.FirstOrDefault(p => p.LessonId == lessonId);
            if (pdf != null)
                pdfRepo.Remove(pdf);

            await _uow.SaveChangesAsync();
        }

        // ────────────────────── Video Content ──────────────────────

        public async Task<VideoLessonResponse> AddVideoAsync(
            int courseId, int lessonId, string teacherId, AddVideoLessonRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);
            var lesson = await GetLessonInCourse(courseId, lessonId);

            // 1:1 rule — check for duplicate
            var videoRepo = _uow.GetRepository<VideoLesson, int>();
            var exists = await videoRepo.AnyAsync(v => v.LessonId == lessonId);
            if (exists)
                throw new ConflictException("This lesson already has a video attached.");

            // Validate URL per provider
            if (!IsValidVideoUrl(request.VideoUrl, request.VideoProvider))
                throw new BadRequestException(
                    $"Invalid URL format for provider '{request.VideoProvider}'.");

            // Auto-generate thumbnail if blank
            var thumbnail = request.ThumbnailUrl
                ?? GenerateThumbnail(request.VideoUrl, request.VideoProvider);

            var video = new VideoLesson
            {
                LessonId = lessonId,
                VideoUrl = request.VideoUrl,
                VideoProvider = request.VideoProvider,
                ThumbnailUrl = thumbnail
            };

            await videoRepo.AddAsync(video);

            // Update lesson type flags
            lesson.Type |= LessonType.Video;
            _uow.GetRepository<Lesson, int>().Update(lesson);

            await _uow.SaveChangesAsync();

            return new VideoLessonResponse
            {
                Id = video.Id,
                LessonId = video.LessonId,
                VideoUrl = video.VideoUrl,
                VideoProvider = video.VideoProvider ?? string.Empty,
                ThumbnailUrl = video.ThumbnailUrl
            };
        }

        public async Task RemoveVideoAsync(
            int courseId, int lessonId, string teacherId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);
            var lesson = await GetLessonInCourse(courseId, lessonId);

            var videoRepo = _uow.GetRepository<VideoLesson, int>();
            var allVideos = await videoRepo.GetAllAsync();
            var video = allVideos.FirstOrDefault(v => v.LessonId == lessonId);

            if (video is null)
                throw new NotFoundException("No video found for this lesson.");

            videoRepo.Remove(video);

            // Clear the Video flag
            lesson.Type &= ~LessonType.Video;
            _uow.GetRepository<Lesson, int>().Update(lesson);

            await _uow.SaveChangesAsync();
        }

        // ────────────────────── PDF Content ──────────────────────

        public async Task<PdfLessonResponse> AddPdfAsync(
            int courseId, int lessonId, string teacherId, AddPdfLessonRequest request, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);
            var lesson = await GetLessonInCourse(courseId, lessonId);

            // 1:1 rule
            var pdfRepo = _uow.GetRepository<PdfLesson, int>();
            var exists = await pdfRepo.AnyAsync(p => p.LessonId == lessonId);
            if (exists)
                throw new ConflictException("This lesson already has a PDF attached.");

            var pdf = new PdfLesson
            {
                LessonId = lessonId,
                FileUrl = request.FileUrl,
                FileSizeKb = request.FileSizeKb
            };

            await pdfRepo.AddAsync(pdf);

            // Update lesson type flags
            lesson.Type |= LessonType.Pdf;
            _uow.GetRepository<Lesson, int>().Update(lesson);

            await _uow.SaveChangesAsync();

            return new PdfLessonResponse
            {
                Id = pdf.Id,
                LessonId = pdf.LessonId,
                FileUrl = pdf.FileUrl,
                FileSizeKb = pdf.FileSizeKb
            };
        }

        public async Task RemovePdfAsync(
            int courseId, int lessonId, string teacherId, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);
            var lesson = await GetLessonInCourse(courseId, lessonId);

            var pdfRepo = _uow.GetRepository<PdfLesson, int>();
            var allPdfs = await pdfRepo.GetAllAsync();
            var pdf = allPdfs.FirstOrDefault(p => p.LessonId == lessonId);

            if (pdf is null)
                throw new NotFoundException("No PDF found for this lesson.");

            pdfRepo.Remove(pdf);

            lesson.Type &= ~LessonType.Pdf;
            _uow.GetRepository<Lesson, int>().Update(lesson);

            await _uow.SaveChangesAsync();
        }

        // ────────────────────── Free Preview ──────────────────────

        public async Task ToggleFreePreviewAsync(
            int courseId, int lessonId, string teacherId, bool isFreePreview, CancellationToken ct)
        {
            await EnsureCourseOwnership(courseId, teacherId);
            var lesson = await GetLessonInCourse(courseId, lessonId);

            lesson.IsFreePreview = isFreePreview;
            _uow.GetRepository<Lesson, int>().Update(lesson);
            await _uow.SaveChangesAsync();
        }

        // ────────────────────── Helpers ──────────────────────

        private async Task EnsureCourseOwnership(int courseId, string teacherId)
        {
            var ownerTeacherId = await _uow.CourseRepository.GetCourseTeacherIdAsync(courseId);
            if (ownerTeacherId is null)
                throw new NotFoundException("Course not found.");
            if (ownerTeacherId != teacherId)
                throw new UnauthorizedException("You are not the owner of this course.");
        }

        private async Task<Lesson> GetLessonInCourse(int courseId, int lessonId)
        {
            var lesson = await _uow.GetRepository<Lesson, int>().GetByIdAsync(lessonId);
            if (lesson is null || lesson.DeletedAt != null)
                throw new NotFoundException("Lesson not found.");

            // Verify lesson belongs to a section in this course
            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(lesson.SectionId);
            if (section is null || section.CourseId != courseId)
                throw new NotFoundException("Lesson does not belong to this course.");

            return lesson;
        }

        private static LessonResponse MapToResponse(Lesson lesson)
        {
            return new LessonResponse
            {
                Id = lesson.Id,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                Type = lesson.Type.ToString(),
                SortOrder = lesson.SortOrder,
                DurationSeconds = lesson.DurationSeconds,
                IsFreePreview = lesson.IsFreePreview,
                CreatedAt = lesson.CreatedAt
            };
        }

        private static bool IsValidVideoUrl(string url, string provider) =>
            provider.ToLower() switch
            {
                "youtube" => url.Contains("youtube.com") || url.Contains("youtu.be"),
                "vimeo" => url.Contains("vimeo.com"),
                "self" => Uri.TryCreate(url, UriKind.Absolute, out _),
                _ => false
            };

        private static string? GenerateThumbnail(string url, string provider)
        {
            if (provider.Equals("youtube", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var uri = new Uri(url);
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    var videoId = query["v"] ?? uri.Segments.Last();
                    return $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                }
                catch { return null; }
            }
            return null;
        }
    }
}
