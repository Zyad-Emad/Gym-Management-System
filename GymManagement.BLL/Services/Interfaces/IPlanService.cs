using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<Result<PlanViewModel>> GetPlanByIdAsync(int PlanId, CancellationToken ct = default);
        Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default);
        Task<Result> ToggleActivationAsync(int PlanId, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int PlanId, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
