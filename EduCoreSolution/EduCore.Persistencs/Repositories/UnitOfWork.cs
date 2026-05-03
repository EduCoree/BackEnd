using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = [];
        private readonly EduCoreDbContext _eduCoreDbContext;

        // 👇 NEW — holds the active transaction (if any)
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(EduCoreDbContext eduCoreDbContext)
        {
            _eduCoreDbContext = eduCoreDbContext;
            CourseRepository = new CourseRepository(eduCoreDbContext);
            QuizRepository = new QuizRepository(_eduCoreDbContext);
            QuizAttemptRepository = new QuizAttemptRepository(_eduCoreDbContext);
            questionRepository = new QuestiionRepository(_eduCoreDbContext);

            EnrollmentRepository = new EnrollmentRepository(_eduCoreDbContext);
            PaymentRepository = new PaymentRepository(_eduCoreDbContext);
            NotificationRepository = new NotificationRepository(_eduCoreDbContext);


            // Forum
            ForumRepository = new ForumRepository(_eduCoreDbContext);

            // Chat
            ChatRepository = new ChatRepository(_eduCoreDbContext);

            // Teacher Payout System
            TeacherEarningRepository = new TeacherEarningRepository(_eduCoreDbContext);
            TeacherInvoiceRepository = new TeacherInvoiceRepository(_eduCoreDbContext);
            PayoutSettingsRepository = new PayoutSettingsRepository(_eduCoreDbContext);
            ReviewRepository = new ReviewRepository(_eduCoreDbContext);
        }

        public IQuizRepository QuizRepository { get; }

        public ICourseRepository CourseRepository { get; }

        public IQuizAttemptRepository QuizAttemptRepository { get; }

        // ── Forum ──
        public IForumRepository ForumRepository { get; }
        public IEnrollmentRepository EnrollmentRepository { get; }
        public IPaymentRepository PaymentRepository { get; }

        // ── Chat ──
        public IChatRepository ChatRepository { get; }

        // Teacher Payout System
        public ITeacherEarningRepository TeacherEarningRepository { get; }
        public ITeacherInvoiceRepository TeacherInvoiceRepository { get; }
        public IPayoutSettingsRepository PayoutSettingsRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        // 👇 NEW — Transaction management
        // The DbContext stays encapsulated inside this class. Services only see
        // the IUnitOfWork methods and have no knowledge of EF Core.

        public async Task BeginTransactionAsync()
        {
            // Guard against nested BeginTransactionAsync (would throw anyway but clearer error)
            if (_currentTransaction is not null)
                throw new InvalidOperationException("A transaction is already active.");

            _currentTransaction = await _eduCoreDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await _currentTransaction.CommitAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction is null)
                return; // idempotent rollback — safe to call even if nothing to rollback

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public INotificationRepository NotificationRepository { get; }

        public IQuestionRepository questionRepository { get; }

        public IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>
        {
            var EntityType = typeof(TEntity);
            if (_repositories.TryGetValue(EntityType, out var repository))
                return (IGenericRepository<TEntity, Tkey>)repository;

            var NewRepo = new GenericRepository<TEntity, Tkey>(_eduCoreDbContext);
            _repositories.Add(EntityType, NewRepo);
            return NewRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _eduCoreDbContext.SaveChangesAsync();
        }


    }
}
