using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategoryAsync(ct: ct);    
            if(sessions == null || !sessions.Any()) return null;
            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                TrainerName = s.Trainer?.Name ?? "No Trainer",
                CategoryName = s.Category?.Name ?? "No Category",
                Description = s.Description,
                EndDate = s.EndDate,
                StartDate = s.StartDate,
            });   
            foreach(var session in mappedSessions)
            {
                var AvailableSlots =  session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
                //N + 1 Problem
            }
            return mappedSessions;
        }
    }
}
