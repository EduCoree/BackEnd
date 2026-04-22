using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services.Helpers;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class QuizService:IQuizService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public QuizService(IMapper mapper,IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<QuizDto> CreateQuizAsync(int courseId,string teacherId,CreateQuizDto request)
        {

            await ValidationHelpers.EnsureCourseAccessAsync(_unitOfWork, courseId, teacherId);
            var quiz = _mapper.Map<Quiz>(request);
                quiz.CourseId = courseId;
                await _unitOfWork.QuizRepository.AddAsync(quiz);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<QuizDto>PublishQuizAsync(int quizId, string teacherId)
        {
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork,quizId, teacherId);
            var hasQuestions = await _unitOfWork.GetRepository<Question,int>().AnyAsync(q=>q.QuizId == quizId);
            if (!hasQuestions) throw new BadRequestException("Cannot publish a quiz without questions.");
            quiz.IsPublished = true;
            _unitOfWork.QuizRepository.Update(quiz);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuizDto>(quiz);

        }

        public async Task<QuizDto> GetQuizByIdAsync(int quizId,string teacherId)
        {
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<PagedResult<QuizDto>> GetQuizzesByCourseAsync(int courseId,string teacherId,PaginationParams pagination)
        {
            await ValidationHelpers.EnsureCourseAccessAsync(_unitOfWork, courseId,teacherId);
            var (items,totalCount) = await _unitOfWork.QuizRepository.GetQuizzesByCourseAsync(courseId,pagination);
            return new PagedResult<QuizDto>
            { 
                Items= _mapper.Map<IEnumerable<QuizDto>>(items),
              PageNumber= pagination.PageNumber,
              TotalCount=totalCount,
              PageSize= pagination.PageSize,
            };
        }
        public async Task<QuizDto> UpdateQuizAsync(int quizId,string teacherId, UpdateQuizDto request)
        {
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork , quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            _mapper.Map(request, quiz);
            _unitOfWork.QuizRepository.Update(quiz);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuizDto>(quiz);
        }
        public async Task DeleteQuizAsync( int quizId,string teacherId)
        {
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            _unitOfWork.QuizRepository.Remove(quiz);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
