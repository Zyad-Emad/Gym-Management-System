using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MembershipRepository :GenericRepository<Membership> , IMembershipRepository
    {
        private readonly GymDbContext _dbContext;

        public MembershipRepository(GymDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Membership>> GetAllMemershipsWithMembersAndPlansAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default)
        {
            var query = _dbContext.Memberships.AsNoTracking().Include(m => m.Member).Include(m => m.Plan);
            return predicate is not null ? await query.Where(predicate).ToListAsync(ct) : await query.ToListAsync(ct); 
        }
    }
}
