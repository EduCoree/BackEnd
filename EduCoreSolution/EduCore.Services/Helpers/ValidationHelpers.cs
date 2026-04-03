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
            if (teacherIdInDb != teacherId)
                throw new UnauthorizedException();
        }
        public static async Task<Quiz> GetQuizOrThrowAsync(IUnitOfWork unitOfWork, int courseId, int quizId, string teacherId)
        {
                await EnsureCourseAccessAsync(unitOfWork, courseId, teacherId);
            var quiz = await unitOfWork.QuizRepository.GetByIdAsync(quizId);
            if (quiz is null || quiz.CourseId != courseId)
                throw new NotFoundException($"Quiz with id {quizId} not found in course {courseId}.");

            return quiz;
        }

        public static async Task<Question> GetQuestionOrThrowAsync(IUnitOfWork unitOfWork, int quizId, int questionId)
        {
            var question = await unitOfWork.GetRepository<Question, int>().GetByIdAsync(questionId);
            if (question is null || question.QuizId != quizId)
                throw new NotFoundException($"Question with id {questionId} not found in quiz {quizId}.");
            return question;
        }
        public static async Task<AnswerOption> GetAnswerOptionOrThrowAsync(IUnitOfWork unitOfWork, int questionId, int optionId)
        {
            var option = await unitOfWork.GetRepository<AnswerOption, int>().GetByIdAsync(optionId);
            if (option is null || option.QuestionId != questionId)
                throw new NotFoundException($"Answer option with id {optionId} not found in question {questionId}.");
            return option;
        }
    }
}
