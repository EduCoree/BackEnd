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
    public class QuestiionRepository : GenericRepository<Question, int>, IQuestionRepository
    {
        public QuestiionRepository(EduCoreDbContext context) : base(context)
        {

        }
        public async Task<Question?> GetQuestionsWithAnswers(int questionId)
        {
            return await _EduCoreDbContext.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId);
                
        }
    }
}
