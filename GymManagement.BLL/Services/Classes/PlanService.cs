using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {

        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                IsActive = p.IsActive,
                DurationDays = p.DurationDays
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan == null) return null;
            return new PlanViewModel()
            {
                Name = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                IsActive = plan.IsActive,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(PlanId, ct)) return null;
            return new UpdatePlanViewModel()
            {
                PlanName = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<bool> ToggleActivationAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipsAsync(PlanId, ct)) return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now; 
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0;
        }

        public async Task<bool> UpdatePlanAsync(int PlanId, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null) return false;
            if(await HasActiveMembershipsAsync(PlanId, ct)) return false;
            plan.DurationDays = model.DurationDays;
            plan.Description = model.Description;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0;
        }
        private async Task<bool> HasActiveMembershipsAsync(int PlanId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == PlanId && m.EndDate > DateTime.Now, ct);
        }
    }
}
