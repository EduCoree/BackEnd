using EduCore.Shared.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByCourseAsync(int courseId);
        Task<(ReviewDto? Review, string? Error)> CreateReviewAsync(int courseId, string studentId, CreateReviewDto dto);
        Task<(ReviewDto? Review, string? Error)> UpdateReviewAsync(int courseId, int reviewId, string studentId, UpdateReviewDto dto);
        Task<(bool Success, string? Error)> DeleteReviewAsync(int courseId, int reviewId, string studentId, bool isAdmin);
        Task<ReviewSummaryDto> GetReviewSummaryAsync(int courseId);
    }
}
