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
        public async Task<QuestionDto> AddQuestionAsync(int quizId,string teacherId, CreateQuestionDto request)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            var question = _mapper.Map<Question>(request);
            question.QuizId = quizId;
            await _unitOfWork.GetRepository<Question, int>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task DeleteQuestionAsync( int quizId, int questionId,string teacherId)
        {
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork,questionId, teacherId,quizId);
            
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            _unitOfWork.GetRepository<Question, int>().Remove(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<QuizDetailsDto> GetQuestionsByQuizAsync(int quizId,string teacherId)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            var quizWithDetails = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            return _mapper.Map<QuizDetailsDto>(quizWithDetails);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int quizId,int questionId,string teacherID, UpdateQuestionDto request)
        {
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork,questionId,teacherID,quizId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
             _mapper.Map(request, question);
             _unitOfWork.GetRepository<Question, int>().Update(question);
             await _unitOfWork.SaveChangesAsync();
             return _mapper.Map<QuestionDto>(question);
           
        }
    }
}
