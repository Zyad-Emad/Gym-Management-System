using AutoMapper;
using GymManagement.BLL.Common;
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
        private readonly IMapper mapper;

        public PlanService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);
        }

        public async Task<Result<PlanViewModel>> GetPlanByIdAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan == null) return Result<PlanViewModel>.NotFound("Plan Not Found");
            var res = mapper.Map<Plan, PlanViewModel>(plan);
            return Result<PlanViewModel>.OK(res);
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null) return Result<UpdatePlanViewModel>.NotFound("Plan Not Found");
            if (!plan.IsActive) return Result<UpdatePlanViewModel>.Fail("Cannot Update InActive Plans");
            if (await HasActiveMembershipsAsync(PlanId, ct)) return Result<UpdatePlanViewModel>.Fail("Cannot Update Plans with Active Memberships");
            var res = mapper.Map<Plan, UpdatePlanViewModel>(plan);
            return Result<UpdatePlanViewModel>.OK(res);
        }

        public async Task<Result> ToggleActivationAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (plan.IsActive && await HasActiveMembershipsAsync(PlanId, ct)) return Result.ValidationFailed("Plan Already has active members");
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now; 
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to Activate Plan");
        }

        public async Task<Result> UpdatePlanAsync(int PlanId, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (await HasActiveMembershipsAsync(PlanId, ct)) return Result.ValidationFailed("Plan Already has active members");
            mapper.Map<UpdatePlanViewModel, Plan>(model, plan);
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to Update Plan");
        }
        private async Task<bool> HasActiveMembershipsAsync(int PlanId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == PlanId && m.EndDate > DateTime.Now, ct);
        }
    }
}
