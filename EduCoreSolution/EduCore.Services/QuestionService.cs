using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Exceptions;
using EduCore.Shared.DTOs.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EduCore.Services.Helpers;
using EduCore.Domain.Entities.QuizModel;

namespace EduCore.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<QuestionDto> AddQuestionAsync(int courseId, int quizId, CreateQuestionDto request)
        {
            await ValidationHelpers.GetCourseOrThrowAsync(_unitOfWork, courseId);
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork,courseId, quizId);
            var question = _mapper.Map<Question>(request);
            question.QuizId = quizId;
            await _unitOfWork.GetRepository<Question, int>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuestionDto>(question);
        }

        public Task DeleteQuestionAsync(int courseId, int quizId, int questionId)
        {
            throw new NotImplementedException();
        }

        public async Task<QuizDetailsDto> GetQuestionsByQuizAsync(int courseId, int quizId)
        {
            await ValidationHelpers.GetCourseOrThrowAsync(_unitOfWork,courseId);
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz is null || quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz with id {quizId} not found in course {courseId}.");

            return _mapper.Map<QuizDetailsDto>(quiz);
        }

        public Task<QuestionDto> UpdateQuestionAsync(int courseId, int quizId, CreateQuestionDto request)
        {
            throw new NotImplementedException();
        }
    }
}
