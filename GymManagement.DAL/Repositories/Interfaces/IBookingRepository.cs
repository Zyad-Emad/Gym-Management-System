using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking> 
    {
        Task<IEnumerable<Booking>> GetBookingBySessionIdAsync(int sessionId, CancellationToken ct = default);
    }
}
