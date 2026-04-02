using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;

namespace EduCore.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // ── Public ────────────────────────────────────────────────

        public async Task<PagedResult<CourseSummaryDto>> GetAllPublishedAsync(
            CourseFilterDto filter, PaginationParams pagination)
        {
            var (courses, totalCount) = await _uow.CourseRepository
                .GetFilteredPagedAsync(filter, pagination);

            return new PagedResult<CourseSummaryDto>
            {
                Items = _mapper.Map<IEnumerable<CourseSummaryDto>>(courses),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<CourseDetailDto> GetCourseByIdAsync(int id)
        {
            var course = await _uow.CourseRepository.GetWithSectionsAsync(id);

            if (course is null)
                throw new NotFoundException("Corse Not Found");

            if (course.Status != CourseStatus.Published)
                throw new NotFoundException("Cours Not Avilable");

            return _mapper.Map<CourseDetailDto>(course);
        }

        // ── Student ───────────────────────────────────────────────

        public async Task<IEnumerable<StudentEnrolledCourseDto>> GetMyCoursesAsync(string studentId)
        {
            return await _uow.CourseRepository.GetStudentEnrolledCoursesAsync(studentId);
        }

        // ── Teacher ───────────────────────────────────────────────

        public async Task<PagedResult<CourseSummaryDto>> GetTeacherCoursesAsync(
            string teacherId, PaginationParams pagination)
        {
            var courses = await _uow.CourseRepository.GetByTeacherAsync(teacherId);
            var total = courses.Count();

            var paged = courses
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            return new PagedResult<CourseSummaryDto>
            {
                Items = _mapper.Map<IEnumerable<CourseSummaryDto>>(paged),
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<CourseDetailDto> GetTeacherCourseByIdAsync(int courseId, string teacherId)
        {
            var course = await _uow.CourseRepository.GetWithSectionsAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            return _mapper.Map<CourseDetailDto>(course);
        }

        public async Task<CourseSummaryDto> CreateCourseAsync(string teacherId, CreateCourseDto dto)
        {
            var course = _mapper.Map<Course>(dto);
            course.TeacherId = teacherId;
            course.Status = CourseStatus.Draft; 

            await _uow.CourseRepository.AddAsync(course);
            await _uow.SaveChangesAsync();

            return _mapper.Map<CourseSummaryDto>(course);
        }

        public async Task<CourseSummaryDto> UpdateCourseAsync(
            int courseId, string teacherId, UpdateCourseDto dto)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            _mapper.Map(dto, course);
            _uow.CourseRepository.Update(course);
            await _uow.SaveChangesAsync();

            return _mapper.Map<CourseSummaryDto>(course);
        }

        public async Task DeleteCourseAsync(int courseId, string teacherId)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            if (await _uow.CourseRepository.HasEnrollmentsAsync(courseId))
                throw new BadRequestException("You can't delete the course — there are enrolled students.");

            _uow.CourseRepository.Remove(course);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdateCoverImageAsync(int courseId, string teacherId, string imageUrl)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            course.CoverImage = imageUrl;
            _uow.CourseRepository.Update(course);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdatePricingAsync(int courseId, string teacherId, UpdatePricingDto dto)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            course.PricingType = dto.PricingType;
            course.Price = dto.Price;
            course.DiscountedPrice = dto.DiscountedPrice;
            _uow.CourseRepository.Update(course);
            await _uow.SaveChangesAsync();
        }

        public async Task PublishCourseAsync(int courseId, string teacherId)
            => await SetStatusAsync(courseId, CourseStatus.Published, teacherId);

        public async Task UnpublishCourseAsync(int courseId, string teacherId)
            => await SetStatusAsync(courseId, CourseStatus.Archived, teacherId);

        // ── Sections ──────────────────────────────────────────────

        public async Task<List<SectionDto>> GetCourseSectionsAsync(int courseId)
        {
            var course = await _uow.CourseRepository.GetWithSectionsAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            return _mapper.Map<List<SectionDto>>(course.Sections);
        }

        public async Task<SectionDto> AddSectionAsync(
            int courseId, string teacherId, CreateSectionDto dto)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            var section = new Section
            {
                CourseId = courseId,
                Title = dto.Title,
                SortOrder = 0
            };

            await _uow.GetRepository<Section, int>().AddAsync(section);
            await _uow.SaveChangesAsync();

            return _mapper.Map<SectionDto>(section);
        }

        public async Task UpdateSectionAsync(
            int courseId, int sectionId, string teacherId, UpdateSectionDto dto)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(sectionId);

            if (section is null || section.CourseId != courseId)
                throw new NotFoundException("Section Not Found");

            section.Title = dto.Title;
            _uow.GetRepository<Section, int>().Update(section);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteSectionAsync(int courseId, int sectionId, string teacherId)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(sectionId);

            if (section is null || section.CourseId != courseId)
                throw new NotFoundException("Section Not Found");

            //  Cascade delete lessons
            _uow.GetRepository<Section, int>().Remove(section);
            await _uow.SaveChangesAsync();
        }

        public async Task ReorderSectionsAsync(
            int courseId, string teacherId, List<ReorderItemDto> items)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            var sectionRepo = _uow.GetRepository<Section, int>();

            foreach (var item in items)
            {
                var section = await sectionRepo.GetByIdAsync(item.Id);
                if (section is null || section.CourseId != courseId) continue;

                section.SortOrder = item.SortOrder;
                sectionRepo.Update(section);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task ReorderLessonsAsync(
            int courseId, int sectionId, string teacherId, List<ReorderItemDto> items)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.TeacherId != teacherId)
                throw new UnauthorizedException();

            var section = await _uow.GetRepository<Section, int>().GetByIdAsync(sectionId);

            if (section is null || section.CourseId != courseId)
                throw new NotFoundException("Section Not Found");

            var lessonRepo = _uow.GetRepository<Lesson, int>();

            foreach (var item in items)
            {
                var lesson = await lessonRepo.GetByIdAsync(item.Id);
                if (lesson is null || lesson.SectionId != sectionId) continue;

                lesson.SortOrder = item.SortOrder;
                lessonRepo.Update(lesson);
            }

            await _uow.SaveChangesAsync();
        }

        // ── Admin ─────────────────────────────────────────────────

        public async Task<PagedResult<CourseSummaryDto>> GetAllCoursesAdminAsync(
            CourseFilterDto filter, PaginationParams pagination)
        {
            var query = _uow.CourseRepository;
            //Draft, Published, Archived
            var all = await _uow.CourseRepository.GetAllAsync();

            if (filter.CategoryId.HasValue)
                all = all.Where(c => c.CategoryId == filter.CategoryId.Value);

            if (filter.Level.HasValue)
                all = all.Where(c => c.Level == filter.Level.Value);

            if (filter.PricingType.HasValue)
                all = all.Where(c => c.PricingType == filter.PricingType.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                all = all.Where(c => c.Title.Contains(filter.Search));

            var total = all.Count();
            var paged = all
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            return new PagedResult<CourseSummaryDto>
            {
                Items = _mapper.Map<IEnumerable<CourseSummaryDto>>(paged),
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task AdminPublishAsync(int courseId)
            => await SetStatusAsync(courseId, CourseStatus.Published);

        public async Task AdminUnpublishAsync(int courseId)
            => await SetStatusAsync(courseId, CourseStatus.Archived);

        public async Task AdminUpdatePricingAsync(int courseId, UpdatePricingDto dto)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            course.PricingType = dto.PricingType;
            course.Price = dto.Price;
            course.DiscountedPrice = dto.DiscountedPrice;
            _uow.CourseRepository.Update(course);
            await _uow.SaveChangesAsync();
        }

        // ── Helper ────────────────────────────────────────────────

        private async Task SetStatusAsync(int courseId, CourseStatus status, string? teacherId = null)
        {
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (teacherId != null && course.TeacherId != teacherId)
                throw new UnauthorizedException();

            course.Status = status;
            _uow.CourseRepository.Update(course);
            await _uow.SaveChangesAsync();
        }
    }
}