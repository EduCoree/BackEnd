using EduCore.Domain.Contracts;
using EduCore.Domain.Entities;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class GenericRepository<TEntity, Tkey> : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        public EduCoreDbContext _EduCoreDbContext { get; }

        public GenericRepository(EduCoreDbContext eduCoreDbContext)
        {
            _EduCoreDbContext = eduCoreDbContext;
        }
        public async Task AddAsync(TEntity entity) => await _EduCoreDbContext.AddAsync(entity);


        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _EduCoreDbContext.Set<TEntity>().ToListAsync();

        public IQueryable<TEntity> GetAllAsQueryable() => _EduCoreDbContext.Set<TEntity>().AsQueryable();

        public async Task<TEntity?> GetByIdAsync(Tkey id) => await _EduCoreDbContext.Set<TEntity>().FindAsync(id);


        public void Remove(TEntity entity) => _EduCoreDbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity) => _EduCoreDbContext.Set<TEntity>().Update(entity);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _EduCoreDbContext.Set<TEntity>().AnyAsync(predicate);
        }
    }
}
