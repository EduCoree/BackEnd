using EduCore.Domain.Entities.CourseModel;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course, int>
    {
        Task<(IEnumerable<Course> Courses, int TotalCount)> GetFilteredPagedAsync(
            CourseFilterDto filter, PaginationParams pagination);
        Task<Course?> GetWithSectionsAsync(int courseId);
        Task<IEnumerable<Course>> GetByTeacherAsync(string teacherId);
        Task<bool> HasEnrollmentsAsync(int courseId);
        Task<IEnumerable<StudentEnrolledCourseDto>> GetStudentEnrolledCoursesAsync(string studentId);
    }
}
