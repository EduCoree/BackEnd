using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services.Helpers;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class StudentQuizService : IstudentQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentQuizService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AttemptHistoryDto>> GetHistoryAsync(string studentId)
        {
            var attempts = await _unitOfWork.QuizAttemptRepository.GetStudentHistoryAsync(studentId);
            return _mapper.Map<IEnumerable<AttemptHistoryDto>>(attempts);
        }

        public async Task<StudentQuizDto> GetQuizAsync(int quizId, string studentId)
        {
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz is null)
                throw new NotFoundException($"Quiz with id {quizId} not found.");
            var usedAttempts = await _unitOfWork.QuizAttemptRepository.GetAttemptCountAsync(quizId, studentId);
            var AttemptsLeft = quiz.MaxAttempts - usedAttempts;
            var quizDto = _mapper.Map<StudentQuizDto>(quiz);
            return quizDto with { AttemptsLeft = AttemptsLeft };
        }

        public async Task<AttemptResultDto> GetResultAsync(int quizId, int attemptId, string studentId)
        {
            var attempt = await _unitOfWork.QuizAttemptRepository.GetAttemptWithAnswersAsync(attemptId);
            if (attempt is null || attempt.QuizId != quizId || attempt.StudentId != studentId)
                throw new NotFoundException($"Attempt with id {attemptId} not found");
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId); 
            var totalPoints = quiz!.Questions.Sum(q => q.Points);
            var EarnedPoints = attempt.AttemptAnswers.Where(a=>a.AnswerOption.IsCorrect).Sum(a => a.Question.Points);
            return BuildResultDto(attempt, EarnedPoints, totalPoints);
        }

        public async Task<AttemptDto> StartAttemptAsync(int quizId, string studentId)
        {
            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz is null)
                throw new NotFoundException($"Quiz with id {quizId} not found.");
            var usedAttempts = await _unitOfWork.QuizAttemptRepository.GetAttemptCountAsync(quizId, studentId);
            if (usedAttempts >= quiz.MaxAttempts)
                throw new BadRequestException("Maximum number of attempts reached for this quiz.");
            var attempt = new QuizAttempt
            { 
                QuizId = quizId,
                StudentId = studentId,
                StartedAt = DateTime.Now,
            };
            await _unitOfWork.QuizAttemptRepository.AddAsync(attempt);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AttemptDto>(attempt);

        }

        public async Task<AttemptResultDto> SubmitAttemptAsync(int quizId, int attemptId, string studentId, SubmitAnswerDto request)
        {
            var attempt= await _unitOfWork.QuizAttemptRepository.GetAttemptWithAnswersAsync(attemptId);
            if (attempt is null || attempt.QuizId != quizId || attempt.StudentId != studentId)
                throw new NotFoundException($"Attempt with id {attemptId} not found");
            if (attempt.SubmittedAt.HasValue)
                throw new BadRequestException("This attempt has already been submitted.");

            var quiz = await _unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            foreach (var answer in request.Answers)
            {
                var question = await ValidationHelpers.GetQuestionOrThrowAsync(_unitOfWork, quizId, answer.QuestionId);
                var attemptAnswer = new AttemptAnswer
                {
                    AttemptId = attemptId,
                    QuestionId = answer.QuestionId,
                    AnswerOptionId = answer.AnswerOptionId
                };
                attempt.AttemptAnswers.Add(attemptAnswer);
            }
            attempt.SubmittedAt = DateTime.Now;
            var TotalPoints = quiz!.Questions.Sum(q => q.Points);
            var EarnedPoints = 0;
            foreach(var answer in request.Answers)
            {
                var question = quiz.Questions.First(q => q.Id == answer.QuestionId);
                var selectedOption = question.AnswerOptions.FirstOrDefault(a => a.Id == answer.AnswerOptionId);
                if ( selectedOption?.IsCorrect ==true)
                    EarnedPoints += question.Points;
            }
            var score = TotalPoints > 0 ? Math.Round((decimal)EarnedPoints / TotalPoints * 100, 2) : 0;
            var passed = score >= quiz.PassScore;
            attempt.Score = score;
            attempt.Passed = passed;
            attempt.SubmittedAt = DateTime.Now;
            _unitOfWork.QuizAttemptRepository.Update(attempt);
            await _unitOfWork.SaveChangesAsync();
            return BuildResultDto(attempt, EarnedPoints, TotalPoints);

        }





        #region Helpers
        private AttemptResultDto BuildResultDto(QuizAttempt attempt, int earnedPoints, int totalPoints)
        {
            var review = attempt.AttemptAnswers.Select(aa =>
            {
                var correctOption = aa.Question.AnswerOptions.FirstOrDefault(o => o.IsCorrect);
                return new QuestionReviewDto
                {
                    QuestionId = aa.QuestionId,
                    QuestionText = aa.Question.Text,
                    SelectedAnswerText = aa.AnswerOption.Text,
                    CorrectAnswerText = correctOption?.Text ?? string.Empty,
                    IsCorrect = aa.AnswerOption.IsCorrect,
                    Points = aa.Question.Points
                };
            }).ToList();

            return new AttemptResultDto
            {
                AttemptId = attempt.Id,
                Score = attempt.Score ?? 0,
                Passed = attempt.Passed,
                SubmittedAt = attempt.SubmittedAt ?? DateTime.UtcNow,
                TotalPoints = totalPoints,
                EarnedPoints = earnedPoints,
                Review = review
            };
        }
        #endregion

    }
}
