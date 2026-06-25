using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _membershipService.GetAllMembershipAsync(ct));
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownAsync(ct);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }
            var result = await _membershipService.CreateMembershipAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownAsync(ct);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id , CancellationToken ct)
        {
            var result = await _membershipService.DeleteActiveMembershipAsync(id, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Membership cancelled" : result.error;
            return RedirectToAction(nameof(Index)); 
        }
        private async Task PopulateDropDownAsync(CancellationToken ct)
        {
            ViewBag.Plans = new SelectList(await _membershipService.GetPlansForDropDownListAsync(ct) , "Id" , "Name");
            ViewBag.Members = new SelectList(await _membershipService.GetMembersForDropDownListAsync(ct), "Id", "Name");
        }
    }
}
