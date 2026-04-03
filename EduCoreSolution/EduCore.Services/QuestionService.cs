using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EduCore.Services.Helpers;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.DTOs.Quiz.Teacher;

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
        public async Task<QuestionDto> AddQuestionAsync(int courseId, int quizId,string teacherId, CreateQuestionDto request)
        {
            var quiz = await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork,courseId, quizId,teacherId);
            var question = _mapper.Map<Question>(request);
            question.QuizId = quizId;
            await _unitOfWork.GetRepository<Question, int>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task DeleteQuestionAsync(int courseId, int quizId, int questionId,string teacherId)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherId);
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, questionId);
            var hasAttempts = await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if (hasAttempts)
                throw new BadRequestException("Cannot delete a question from a quiz that already has attempts.");
            _unitOfWork.GetRepository<Question, int>().Remove(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<QuizDetailsDto> GetQuestionsByQuizAsync(int courseId, int quizId,string teacherId)
        {
           await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherId);
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz is null || quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz with id {quizId} not found in course {courseId}.");

            return _mapper.Map<QuizDetailsDto>(quiz);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int courseId, int quizId,int questionId,string teacherID, UpdateQuestionDto request)
        {
             await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherID);
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, questionId);
             var hasAttempts = await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
             if (hasAttempts)
                 throw new BadRequestException("Cannot update a question in a quiz that already has attempts.");
             _mapper.Map(request, question);
             _unitOfWork.GetRepository<Question, int>().Update(question);
             await _unitOfWork.SaveChangesAsync();
             return _mapper.Map<QuestionDto>(question);
           
        }
    }
}
