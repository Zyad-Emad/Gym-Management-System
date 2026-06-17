using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(GymDbContext gymDbContext)
        {
            _dbContext = gymDbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }
        public async Task<int> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }
        public async Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = (tracking ? _dbSet : _dbSet.AsNoTracking());
            return await query.ToListAsync(ct);
        }
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) => await _dbSet.FindAsync([id] , ct); // Note : dont write FindAsync(id , ct) as it will consider ct a part of key values

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _dbSet : _dbSet.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}
