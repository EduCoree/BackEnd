using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Dtos.Quiz;
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

        public async Task<QuizDto> CreateQuizAsync(int courseId,CreateQuizDto request)
        {
          
                var courseExists = await _unitOfWork.GetRepository<Course, int>().GetByIdAsync(courseId);
                if (courseExists == null)
                throw new NotFoundException($"Course with id {courseId} not found.");
                var quiz = _mapper.Map<Quiz>(request);
                quiz.CourseId = courseId;
                await _unitOfWork.QuizRepository.AddAsync(quiz);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<QuizDto> GetQuizByIdAsync(int courseId,int quizId)
        {
            var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
            if (quiz is null || quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz {quizId} not found in CourseId {courseId}.");
            return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId)
        {
            var course =await _unitOfWork.GetRepository<Course, int>().GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException($"Course with id {courseId} not found.");
            var quizzes = await _unitOfWork.QuizRepository.GetQuizzesByCourseAsync(courseId);
            return _mapper.Map<IEnumerable<QuizDto>>(quizzes);

        }
        public async Task<QuizDto> UpdateQuizAsync(int courseId,int quizId, CreateQuizDto request)
        {
            var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
            if (quiz is null || quiz.CourseId!= courseId)
                throw new NotFoundException($"Quiz {quizId} not found in CourseId {courseId}.");
            var hasAttempts =await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if(hasAttempts)
                throw new BadRequestException("Cannot update a quiz that already has attempts.");
            _mapper.Map(request, quiz);
            _unitOfWork.QuizRepository.Update(quiz);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<QuizDto>(quiz);
        }

        public async Task DeleteQuizAsync(int courseId, int quizId)
        {
            var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
            if (quiz is null || quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz {quizId} not found in CourseId {courseId}.");
            var hasAttempts = await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if (hasAttempts)
                throw new BadRequestException("Cannot update a quiz that already has attempts.");
            _unitOfWork.QuizRepository.Remove(quiz);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
