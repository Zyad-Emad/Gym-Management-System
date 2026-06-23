using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize]

    public class SessionsController : Controller
    {
        private readonly ISessionService sessionService;

        public SessionsController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var sessions = await sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }
        #region Create Session
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(model);
            }
            var result = await sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownsAsync();
            return View(model);
        }
        private async Task PopulateDropDownsAsync(CancellationToken ct = default)
        {
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await sessionService.GetCategoriesForDropDownAsync(ct), "Id", "Name");
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct = default)
        {
            var res = await sessionService.GetSessionByIdAsync(id, ct);
            if (res.success)
            {
                return View(res.data);
            }
            TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct = default)
        {
            var res = await sessionService.GetSessionToUpdateAsync(id , ct);
            if (res.success)
            {
                ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
                return View(res.data);
            }
            TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdateSessionViewModel model , CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
                return View(model);
            }
            var result = await sessionService.UpdateSessionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
            return View(model);
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct = default)
        {
            var res = await sessionService.GetSessionByIdAsync(id, ct);
            if (res.success)
                return View(res.data);
            
            TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct = default)
        {
            var result = await sessionService.RemoveSessionAsync(id , ct) ;
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
