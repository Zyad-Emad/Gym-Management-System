using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
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
            if (res)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Trainer Failed to create.";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var trainer = await trainerService.GetTrainerDetailsAsync(id, ct);
            if(trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var trainer = await trainerService.GetTrainerToUpdateAsync(id, ct);
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id , TrainerToUpdateViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var res = await trainerService.UpdateTrainerDetailsAsync(id , model , ct);
            if (res)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else TempData["ErrorMessage"] = "Trainer Failed To Update";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var trainer = await trainerService.GetTrainerDetailsAsync(id, ct);
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct)
        {
            var res = await trainerService.RemoveTrainerAsync(id, ct);
            if (res) TempData["SuccessMessage"] = "Trainer Deleted Successfully.";
            else TempData["ErrorMessage"] = "Failed to delete Trainer.";
            return RedirectToAction(nameof(Index));
        }


    }
}
