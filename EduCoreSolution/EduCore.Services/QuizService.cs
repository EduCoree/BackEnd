using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services.Helpers;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
             await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork,quizId, teacherId);
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz.IsPublished)
                throw new BadRequestException("Quiz is already published");
            ValidateQuizReadyToPublish(quiz);
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

        #region helpers
        public static void ValidateQuizReadyToPublish(Quiz quiz)
        {
            if (!quiz.Questions.Any())
                throw new BadRequestException("Quiz must have at least one question");

            foreach (var question in quiz.Questions)
            {
                if (question.AnswerOptions == null || !question.AnswerOptions.Any())
                    throw new BadRequestException("Answer options are required");
                var correctCount = question.AnswerOptions.Count(o => o.IsCorrect);
                if (correctCount == 0)
                    throw new BadRequestException("At least one answer option must be marked as correct.");

                switch (question.Type)
                {
                    case QuestionType.MCQ:
                        if (question.AnswerOptions.Count < 2)
                            throw new BadRequestException($"Question '{question.Text}' must have at least 2 options");
                        if (question.AnswerOptions.Count(o => o.IsCorrect) != 1)
                            throw new BadRequestException($"Question '{question.Text}' must have exactly 1 correct answer");
                        break;

                    case QuestionType.TrueFalse:
                        if (question.AnswerOptions.Count != 2)
                            throw new BadRequestException($"Question '{question.Text}' must have exactly 2 options");
                        if (question.AnswerOptions.Count(o => o.IsCorrect) != 1)
                            throw new BadRequestException($"Question '{question.Text}' must have exactly 1 correct answer");
                        break;
                }
            }
        }

  

        #endregion

    }
}
