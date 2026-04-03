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
        public async Task<AnswerOptionDto> AddAnswerOptionAsync(int courseId, int quizId, int questionId,string teacherId, CreateAnswerOptionDto request)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherId);
            await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, questionId);
            var option = _mapper.Map<AnswerOption>(request);
            option.QuestionId = questionId;
            await _unitOfWork.GetRepository<AnswerOption,int>().AddAsync(option);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AnswerOptionDto>(option);
        }

        public async Task DeleteAnswerOptionAsync(int courseId, int quizId, int questionId, int optionId,string teacherId)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherId);
            await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, questionId);
            var option = await ValidationHelpers.GetAnswerOptionOrThrowAsync(_unitOfWork, questionId, optionId);
            var hasAttempts = await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if(hasAttempts)
                throw new BadRequestException("Cannot delete an answer option that is part of a quiz with attempts.");
            _unitOfWork.GetRepository<AnswerOption, int>().Remove(option);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<AnswerOptionDto> UpdateAnswerOptionAsync(int courseId, int quizId, int questionId, int optionId,string teacherId, UpdateAnswerOptionDto request)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, courseId, quizId,teacherId);
            await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, questionId);
            var option = await ValidationHelpers.GetAnswerOptionOrThrowAsync(_unitOfWork, questionId, optionId);

            var hasAttempts = await _unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if (hasAttempts)
                throw new BadRequestException("Cannot update an answer option in a quiz that already has attempts.");

            _mapper.Map(request, option);
            _unitOfWork.GetRepository<AnswerOption, int>().Update(option);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AnswerOptionDto>(option);
        }
    }
}
