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
using EduCore.Shared.Enums;

namespace EduCore.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<QuestionDto> AddQuestionAsync(int quizId, string teacherId, CreateQuestionDto request)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            ValidationHelpers.ValidateAnswerOptions(request.AnswerOptions.Count,request.AnswerOptions.Count(o => o.IsCorrect),request.Type);
            var question = _mapper.Map<Question>(request);
            question.QuizId = quizId;

            await _unitOfWork.GetRepository<Question, int>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<QuestionDto>(question);
        }



        public async Task DeleteQuestionAsync(int quizId, int questionId, string teacherId)
        {
            var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, questionId, teacherId, quizId);

            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            _unitOfWork.GetRepository<Question, int>().Remove(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<QuizDetailsDto> GetQuestionsByQuizAsync(int quizId, string teacherId)
        {
            var quizWithDetails = await ValidationHelpers.GetQuizWithDetailsOrThrowAsync(_unitOfWork, quizId, teacherId);
            return _mapper.Map<QuizDetailsDto>(quizWithDetails);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int quizId, int questionId, string teacherID, UpdateQuestionDto request)
        {
            var question = await ValidationHelpers.GetQuestionWithAnswersOrThrowAsync(_unitOfWork, questionId, teacherID, quizId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            ValidationHelpers.ValidateAnswerOptions(request.AnswerOptions.Count, request.AnswerOptions.Count(o => o.IsCorrect), request.Type);
            _mapper.Map(request, question);
            var ExitingOptions = question.AnswerOptions;
            var incomingIds = request.AnswerOptions
           .Where(o => o.Id.HasValue)
           .Select(o => o.Id!.Value)
           .ToList();

            // delete options removed by teacher
            var toDelete = ExitingOptions.Where(o => !incomingIds.Contains(o.Id));
            foreach (var option in toDelete)
                _unitOfWork.GetRepository<AnswerOption, int>().Remove(option);

            // update existing or add new
            foreach (var optionDto in request.AnswerOptions)
            {
                if (optionDto.Id.HasValue)
                {
                    // existing option → update it
                    var existing = ExitingOptions.First(o => o.Id == optionDto.Id);
                    _mapper.Map(optionDto, existing);
                    _unitOfWork.GetRepository<Question, int>().Update(question);

                }
                else
                {
                    // no id → new option
                    var newOption = _mapper.Map<AnswerOption>(optionDto);
                    newOption.QuestionId = questionId;
                    await _unitOfWork.GetRepository<AnswerOption, int>().AddAsync(newOption);
                }
            }


            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuestionDto>(question);

        }


    }
    }
