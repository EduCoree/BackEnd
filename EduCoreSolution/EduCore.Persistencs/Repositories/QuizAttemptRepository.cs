using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class QuizAttemptRepository : GenericRepository<QuizAttempt, int>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(EduCoreDbContext context):base(context)
        {
            
        }
        public async Task<int> GetAttemptCountAsync(int quizId, string studentId)
        {
            return await _EduCoreDbContext.QuizAttempts.CountAsync(a => a.QuizId == quizId && a.StudentId == studentId);
        }
    }
}
