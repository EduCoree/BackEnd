using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
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
            //if (teacherIdInDb != teacherId)
            //    throw new UnauthorizedException();
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
        public static async Task<AnswerOption> GetAnswerOptionOrThrowAsync(IUnitOfWork unitOfWork, int optionId, string teacherId,int? questionId=null)
        {
            var option = await unitOfWork.GetRepository<AnswerOption, int>().GetByIdAsync(optionId);
            if (option is null)
                throw new NotFoundException($"Answer option with id {optionId} not found");
            if (questionId.HasValue && option.QuestionId != questionId)
                throw new NotFoundException("Option not found in this question");
            await GetQuestionOrThrowAsync(unitOfWork,option.QuestionId, teacherId);
            return option;
        }
        public static async Task EnsureNoAttemptsAsync(IUnitOfWork unitOfWork, int quizId)
        {
            var hasAttempts = await unitOfWork.QuizRepository.HasAttemptsAsync(quizId);
            if (hasAttempts)
                throw new BadRequestException("Cannot modify - quiz already has attempts.");
        }

    }
}
