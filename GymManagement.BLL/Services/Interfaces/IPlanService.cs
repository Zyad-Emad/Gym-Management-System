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
        Task<PlanViewModel?> GetPlanByIdAsync(int PlanId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default);
        Task<bool> ToggleActivationAsync(int PlanId, CancellationToken ct = default);
        Task<bool> UpdatePlanAsync(int PlanId, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
