using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService planService;

        public PlansController(IPlanService planService)
        {
            this.planService = planService;
        }
        //Index Action
        // GET BaseUrl/Plans/Index -> Listing ALl Plans
        public async Task<IActionResult> Index(CancellationToken ct) =>
            View(await planService.GetAllPlansAsync(ct));
        //Details Action
        // Get BaseUrl/Plans/Details/1
        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var res = await planService.GetPlanByIdAsync(id , ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);
        }
        // Get BaseUrl/Plans/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var res = await planService.GetPlanToUpdateAsync(id , ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);
        }
        // Post BaseUrl/Plans/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdatePlanViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await planService.UpdatePlanAsync(id , model , ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Plan Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id , CancellationToken ct)
        {
            var result = await planService.ToggleActivationAsync(id , ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Plan Activation Toggled Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
