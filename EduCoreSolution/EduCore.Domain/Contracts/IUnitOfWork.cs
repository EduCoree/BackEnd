using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts
{
    public interface IUnitOfWork
    {
        IEnrollmentRepository EnrollmentRepository { get; }
        IQuizRepository QuizRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IQuizAttemptRepository QuizAttemptRepository { get; }
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>;







    
        IPaymentRepository PaymentRepository { get; }
        ICourseRepository CourseRepository { get; }

        // ── Forum ──
        IForumRepository ForumRepository { get; }

        // ── Chat ──
        IChatRepository ChatRepository { get; }
    }
}
