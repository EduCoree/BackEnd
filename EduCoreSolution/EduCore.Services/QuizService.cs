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
          
                var courseExists = _unitOfWork.GetRepository<Course, int>().GetByIdAsync(courseId);
                if (courseExists == null)
                throw new NotFoundException($"Course with id {courseId} not found.");
                var quiz = _mapper.Map<Quiz>(request);
                quiz.CourseId = courseId;
                await _unitOfWork.QuizRepository.AddAsync(quiz);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<QuizDto> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz == null)
                throw new NotFoundException ($"Quiz with id {quizId} not found.");
            return _mapper.Map<QuizDto>(quiz);
        }
    }
}
