using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities;
using EduCore.Persistencs.Data.DbContexts;
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

        public UnitOfWork(EduCoreDbContext eduCoreDbContext)
        {
            _eduCoreDbContext = eduCoreDbContext;
        }

        public IQuizRepository QuizRepository{ get; }

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
