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
    }
}
