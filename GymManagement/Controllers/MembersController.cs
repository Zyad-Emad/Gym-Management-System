using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService attachmentService;

        public MembersController(IMemberService memberService , IAttachmentService attachmentService)
        {
            _memberService = memberService;
            this.attachmentService = attachmentService;
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var memberResult = await _memberService.GetMemberDetailsByIdAsync(id);
            if (!memberResult.success)
                return NotFound(memberResult.error);
            if (string.IsNullOrWhiteSpace(memberResult.data!.Photo))
                return NotFound(memberResult.error);
            
            var fileResult = attachmentService.GetFile(memberResult.data!.Photo , "MembersPhoto");
            if (!fileResult.success)
                return NotFound(fileResult.error);
            return File(fileResult.data.stream, fileResult.data.contentType);
        }
        #endregion

        //GET BaseUrl/Members/Index
        //Index - List all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }
        //GET BaseUrl/Members/MemberDetails/{id}
        //MemberDetails - Show one member's details 
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var res = await _memberService.GetMemberDetailsByIdAsync(id , ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);

        }

        //GET BaseUrl/Members/HealthRecordDetails/{id}
        //HealthRecordDetails - Show one member's details 
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            //Get HealthRecord By MemberId
            var result = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.data);
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
            if (res.success)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Edit Member
        // GET BaseUrl/Members/EditMember/{id}
        // Edit - Display edit form 
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct = default)
        {
            var res = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View(res.data);
        }
        // POST BaseUrl/Members/EditMember {Member}
        // Edit - Submit Form 

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var res = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (res.success)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = res.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Delete Member
        // Get BaseUrl/Members/DeleteMember/{id}
        // Delete - Display delete confirmation
        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct = default)
        {
            var res = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if(!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var res = await _memberService.RemoveMemberAsync(id, ct);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
            }
            return RedirectToAction(nameof(Index));
        }
        // Post BaseUrl/Members/DeleteMember/{id}
        // Delete - Submit delete request
        #endregion

    }
}
