using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Reviews;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseAsync(int courseId)
        {
            var repo = _unitOfWork.GetRepository<CourseReview, int>();
            var reviews = await repo.GetAllAsync();
            var filtered = reviews.Where(r => r.CourseId == courseId)
                                  .OrderByDescending(r => r.CreatedAt);
            return _mapper.Map<IEnumerable<ReviewDto>>(filtered);
        }



        public async Task<(ReviewDto? Review, string? Error)> CreateReviewAsync(int courseId, string studentId, CreateReviewDto dto)
        {
            
            var enrollmentRepo = _unitOfWork.GetRepository<Enrollment, int>();
            var enrollments = await enrollmentRepo.GetAllAsync();
            var hasEnrollment = enrollments.Any(e => e.CourseId == courseId
                                                  && e.StudentId == studentId
                                                  && e.Status == EnrollmentStatus.Active);
            if (!hasEnrollment)
                return (null, "No active enrollment found for this course.");

           
            var reviewRepo = _unitOfWork.GetRepository<CourseReview, int>();
            var reviews = await reviewRepo.GetAllAsync();
            var alreadyReviewed = reviews.Any(r => r.CourseId == courseId && r.StudentId == studentId);
            if (alreadyReviewed)
                return (null, "You have already reviewed this course.");

            var review = new CourseReview
            {
                CourseId = courseId,
                StudentId = studentId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await reviewRepo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<ReviewDto>(review), null);
        }



        public async Task<(ReviewDto? Review, string? Error)> UpdateReviewAsync(int courseId, int reviewId, string studentId, UpdateReviewDto dto)
        {
            var repo = _unitOfWork.GetRepository<CourseReview, int>();
            var review = await repo.GetByIdAsync(reviewId);

            
            if (review is null || review.CourseId != courseId)
                return (null, "Review not found.");

            if (review.StudentId != studentId)
                return (null, "You can only edit your own review.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<ReviewDto>(review), null);
        }







        public async Task<(bool Success, string? Error)> DeleteReviewAsync(int courseId, int reviewId, string studentId, bool isAdmin)
        {
            var repo = _unitOfWork.GetRepository<CourseReview, int>();
            var review = await repo.GetByIdAsync(reviewId);

          
            if (review is null || review.CourseId != courseId)
                return (false, "Review not found.");

           
            if (!isAdmin && review.StudentId != studentId)
                return (false, "You can only delete your own review.");

            repo.Remove(review);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }


        public async Task<ReviewSummaryDto> GetReviewSummaryAsync(int courseId)
        {
            var repo = _unitOfWork.GetRepository<CourseReview, int>();
            var reviews = await repo.GetAllAsync();
            var courseReviews = reviews.Where(r => r.CourseId == courseId).ToList();

            if (!courseReviews.Any())
                return new ReviewSummaryDto
                {
                    AverageRating = 0,
                    TotalReviews = 0,
                    Distribution = new Dictionary<byte, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            }
                };

           
            var average = Math.Round(courseReviews.Average(r => r.Rating), 1);

           
            var distribution = new Dictionary<byte, int>
    {
        { 1, courseReviews.Count(r => r.Rating == 1) },
        { 2, courseReviews.Count(r => r.Rating == 2) },
        { 3, courseReviews.Count(r => r.Rating == 3) },
        { 4, courseReviews.Count(r => r.Rating == 4) },
        { 5, courseReviews.Count(r => r.Rating == 5) }
    };

            return new ReviewSummaryDto
            {
                AverageRating = average,
                TotalReviews = courseReviews.Count,
                Distribution = distribution
            };
        }

    }
}