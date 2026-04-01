using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ICourseService
    {
        // ── Public ────────────────────────────────────
        Task<PagedResult<CourseSummaryDto>> GetAllPublishedAsync(
            CourseFilterDto filter, PaginationParams pagination);
        Task<CourseDetailDto> GetCourseByIdAsync(int id);

        // ── Student ───────────────────────────────────
        Task<IEnumerable<StudentEnrolledCourseDto>> GetMyCoursesAsync(string studentId);

        // ── Teacher ───────────────────────────────────
        Task<PagedResult<CourseSummaryDto>> GetTeacherCoursesAsync(
            string teacherId, PaginationParams pagination);
        Task<CourseDetailDto> GetTeacherCourseByIdAsync(int courseId, string teacherId);
        Task<CourseSummaryDto> CreateCourseAsync(string teacherId, CreateCourseDto dto);
        Task<CourseSummaryDto> UpdateCourseAsync(int courseId, string teacherId, UpdateCourseDto dto);
        Task DeleteCourseAsync(int courseId, string teacherId);
        Task UpdateCoverImageAsync(int courseId, string teacherId, string imageUrl);
        Task UpdatePricingAsync(int courseId, string teacherId, UpdatePricingDto dto);
        Task PublishCourseAsync(int courseId, string teacherId);
        Task UnpublishCourseAsync(int courseId, string teacherId);

        // ── Sections ──────────────────────────────────
        Task<List<SectionDto>> GetCourseSectionsAsync(int courseId);
        Task<SectionDto> AddSectionAsync(int courseId, string teacherId, CreateSectionDto dto);
        Task UpdateSectionAsync(int courseId, int sectionId, string teacherId, UpdateSectionDto dto);
        Task DeleteSectionAsync(int courseId, int sectionId, string teacherId);
        Task ReorderSectionsAsync(int courseId, string teacherId, List<ReorderItemDto> items);
        Task ReorderLessonsAsync(int courseId, int sectionId, string teacherId, List<ReorderItemDto> items);

        // ── Admin ─────────────────────────────────────
        Task<PagedResult<CourseSummaryDto>> GetAllCoursesAdminAsync(
            CourseFilterDto filter, PaginationParams pagination);
        Task AdminPublishAsync(int courseId);
        Task AdminUnpublishAsync(int courseId);
        Task AdminUpdatePricingAsync(int courseId, UpdatePricingDto dto);
    }
}
