using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.Helpers
{
    public static class ValidationHelpers
    {

        public static async Task EnsureCourseAccessAsync(IUnitOfWork unitOfWork, int courseId, string teacherId)
        {
            var teacherIdInDb = await unitOfWork.CourseRepository.GetCourseTeacherIdAsync(courseId);
            if (teacherIdInDb is null)
                throw new NotFoundException($"Course with id {courseId} not found.");
            if (teacherIdInDb != teacherId)
                throw new UnauthorizedException();
        }
        public static async Task<Quiz> GetQuizOrThrowAsync(IUnitOfWork unitOfWork ,int quizId,string teacherId,int? courseId=null)
        {
            var quiz = await unitOfWork.QuizRepository.GetByIdAsync(quizId);
            if (quiz is null )
                throw new NotFoundException($"Quiz with id {quizId} not found ");
            if (courseId.HasValue && quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz not found in course {courseId}");
            await EnsureCourseAccessAsync(unitOfWork, quiz.CourseId, teacherId);
            return quiz;
        }
        public static async Task<Quiz> GetQuizWithDetailsOrThrowAsync(IUnitOfWork unitOfWork, int quizId, string teacherId, int? courseId = null)
        {
            var quiz = await unitOfWork.QuizRepository.GetQuizWithDetails(quizId);
            if (quiz is null)
                throw new NotFoundException($"Quiz with id {quizId} not found ");
            if (courseId.HasValue && quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz not found in course {courseId}");
            await EnsureCourseAccessAsync(unitOfWork, quiz.CourseId, teacherId);
            return quiz;
        }

        public static async Task<Question> GetQuestionOrThrowAsync(IUnitOfWork unitOfWork, int questionId,string teacherId,int? quizId=null)
        {
            var question = await unitOfWork.GetRepository<Question, int>().GetByIdAsync(questionId);
            if (question is null)
                throw new NotFoundException($"Question with id {questionId} not found");
            if (quizId.HasValue && question.QuizId != quizId)
                throw new NotFoundException($"Question {questionId} not found in quiz {quizId}");
            await GetQuizOrThrowAsync(unitOfWork,question.QuizId, teacherId);
            return question;
        }
        public static async Task<Question> GetQuestionWithAnswersOrThrowAsync(IUnitOfWork unitOfWork, int questionId, string teacherId, int? quizId = null)
        {
            var question = await unitOfWork.questionRepository.GetQuestionsWithAnswers(questionId);
            if (question is null)
                throw new NotFoundException($"Question with id {questionId} not found");
            if (quizId.HasValue && question.QuizId != quizId)
                throw new NotFoundException($"Question {questionId} not found in quiz {quizId}");
            await GetQuizOrThrowAsync(unitOfWork, question.QuizId, teacherId);
            return question;
        }
        public static async Task EnsureNoAttemptsAsync(IUnitOfWork unitOfWork, int quizId)
        {
            var hasAttempts = await unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if (hasAttempts)
                throw new BadRequestException("Cannot modify - quiz already has attempts.");
        }
        public static void ValidateAnswerOptions(int optionCount, int correctCount, QuestionType type)
        {
            if (optionCount == 0)
                throw new BadRequestException("Answer options are required");
            if (correctCount == 0)
                throw new BadRequestException("At least one answer option must be marked as correct.");

            switch (type)
            {
                case QuestionType.MCQ:
                    if (optionCount < 2)
                        throw new BadRequestException("MCQ must have at least 2 options");
                    if (correctCount != 1)
                        throw new BadRequestException("MCQ must have exactly 1 correct answer");
                    break;
                case QuestionType.TrueFalse:
                    if (optionCount != 2)
                        throw new BadRequestException("True/False must have exactly 2 options");
                    if (correctCount != 1)
                        throw new BadRequestException("True/False must have exactly 1 correct answer");

                    break;
            }
        }



    }
}
