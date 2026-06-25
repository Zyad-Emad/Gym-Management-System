using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
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
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings
                .AsNoTracking().CountAsync(b => b.SessionId == sessionId, ct);
        }

        public async Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? predicate = null , CancellationToken ct = default)
        {
            IQueryable<Session> query = _dbContext.Sessions
                .AsNoTracking()
                .Include(s => s.Trainer)
                .Include(s => s.Category);

            if (predicate is not null) query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

        public async Task<Session?> GetSessionWithTrainerAndCategoryByIdAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking()
                .Include(s => s.Trainer)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);
        }
    }
}
