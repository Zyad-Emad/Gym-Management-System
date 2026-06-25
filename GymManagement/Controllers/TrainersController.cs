using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize]

    public class TrainersController : Controller
    {
        private readonly ITrainerService trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            this.trainerService = trainerService;
        }
        public async Task<IActionResult> Index(CancellationToken ct) =>
            View(await trainerService.GetAllTrainersAsync(ct));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var res = await trainerService.CreateTrainerAsync(model, ct);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var res = await trainerService.GetTrainerDetailsAsync(id, ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var res = await trainerService.GetTrainerToUpdateAsync(id, ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id , TrainerToUpdateViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var res = await trainerService.UpdateTrainerDetailsAsync(id , model , ct);
            if (res.success)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var res = await trainerService.GetTrainerDetailsAsync(id, ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct)
        {
            var res = await trainerService.RemoveTrainerAsync(id, ct);
            if (res.success) TempData["SuccessMessage"] = "Trainer Deleted Successfully.";
            else TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }


    }
}
