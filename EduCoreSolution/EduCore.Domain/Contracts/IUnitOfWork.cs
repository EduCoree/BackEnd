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


        IReviewRepository ReviewRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ICourseRepository CourseRepository { get; }

        // ── Forum ──
        IForumRepository ForumRepository { get; }

        // ── Chat ──
        IChatRepository ChatRepository { get; }


        // Teacher Payout System
        ITeacherEarningRepository TeacherEarningRepository { get; }
        ITeacherInvoiceRepository TeacherInvoiceRepository { get; }
        IPayoutSettingsRepository PayoutSettingsRepository { get; }
        // 👇 NEW — Transaction support (abstracts DbContext.Database.BeginTransactionAsync)
        // Allows services to wrap multiple SaveChangesAsync calls in one transaction
        // without depending directly on EF Core's DbContext from the Domain layer.

        /// <summary>
        /// Begins a database transaction. Multiple SaveChangesAsync calls while a
        /// transaction is active are NOT committed to the DB until CommitTransactionAsync
        /// is called. If RollbackTransactionAsync is called (or the transaction is disposed
        /// without commit), all changes made since BeginTransactionAsync are rolled back.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the currently active transaction. Safe to call only after BeginTransactionAsync.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the currently active transaction. Safe to call only after BeginTransactionAsync.
        /// </summary>
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>;
    }
}
