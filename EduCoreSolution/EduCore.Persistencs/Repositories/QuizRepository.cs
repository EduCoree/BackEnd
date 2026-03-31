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
    public class QuizRepository : GenericRepository<Quiz, int>, IQuizRepository
    {
        public QuizRepository(EduCoreDbContext context) : base(context)
        {
        }
        public async Task<Quiz?> GetQuizWithDetails(int quizId)
        {
            return await _EduCoreDbContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }
    }
}
