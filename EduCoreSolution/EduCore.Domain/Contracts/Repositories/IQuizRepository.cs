using EduCore.Domain.Entities.QuizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IQuizRepository:IGenericRepository<Quiz,int>
    {
        Task<Quiz?> GetQuizWithDetails(int quizId);
    }
}
