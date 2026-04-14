using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services.Helpers;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AnswerOptionService : IAnswerOptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnswerOptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AnswerOptionDto> AddAnswerOptionAsync( int questionId,string teacherId, CreateAnswerOptionDto request)
        {
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, questionId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, question.QuizId);
            var option = _mapper.Map<AnswerOption>(request);
            option.QuestionId = questionId;
            await _unitOfWork.GetRepository<AnswerOption,int>().AddAsync(option);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AnswerOptionDto>(option);
        }

        public async Task DeleteAnswerOptionAsync( int questionId, int optionId,string teacherId)
        {
            var option = await ValidationHelpers.GetAnswerOptionOrThrowAsync(_unitOfWork, optionId, teacherId,questionId);
            var question = await _unitOfWork.GetRepository<Question, int>().GetByIdAsync(questionId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, question!.QuizId);
            _unitOfWork.GetRepository<AnswerOption, int>().Remove(option);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<AnswerOptionDto> UpdateAnswerOptionAsync( int questionId, int optionId,string teacherId, UpdateAnswerOptionDto request)
        {
            var option = await ValidationHelpers.GetAnswerOptionOrThrowAsync(_unitOfWork, optionId, teacherId,questionId);
            var question = await _unitOfWork.GetRepository<Question, int>().GetByIdAsync(questionId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, question!.QuizId);
            _mapper.Map(request, option);
            _unitOfWork.GetRepository<AnswerOption, int>().Update(option);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AnswerOptionDto>(option);
        }
    }
}
