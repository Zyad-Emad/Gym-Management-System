using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        //GET BaseUrl/Members/Index
        //Index - List all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        #region Create Member
        //GET BaseUrl/Members/Create
        // Create - show empty form
        [HttpGet]
        public IActionResult Create() => View();
        // Post BaseUrl/Members/Create {Member}
        // CreateMember - Submit Form
        [HttpPost]
        public async Task<IActionResult> CreateMemberAsync(CreateMemberViewModel model , CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(nameof(Create) , model);
            var res = await _memberService.CreateMemberAsync(model, ct);
            if (res)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
