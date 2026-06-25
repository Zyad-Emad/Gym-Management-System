using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcomingSessions = await unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > now , ct);
            var ongoingSessions = await unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= now && s.EndDate >= now, ct);
            var completedSessions = await unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < now, ct);

            var totalMembers = await unitOfWork.GetRepository<Member>().CountAsync(ct : ct);
            var totalTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMembers = await unitOfWork.GetRepository<Membership>().CountAsync(x => x.EndDate > now, ct);

            return new AnalyticsViewModel
            {
                UpcomingSession = upcomingSessions,
                OngoingSession  = ongoingSessions,
                CompletedSession = completedSessions ,
                TotalMembers = totalMembers ,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers
            };
        }
    }
}
