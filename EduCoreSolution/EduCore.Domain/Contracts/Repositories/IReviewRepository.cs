using EduCore.Domain.Entities.ProgressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IReviewRepository : IGenericRepository<CourseReview, int> 
    {
        Task<IEnumerable<CourseReview>> GetReviewsByTeacherAsync(string teacherId, int? courseId = null, int? minRating = null);
        Task<IEnumerable<CourseReview>> GetReviewsByStudentAsync(string studentId);
    }
}
