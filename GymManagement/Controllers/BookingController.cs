using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _bookingService.GetAllSessionsAsync(ct));

        [HttpGet]
        public async Task<IActionResult> Create(int id , CancellationToken ct)
        {
            var members = await _bookingService.GetMemberForDropDownAsync(id, ct);
            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId =id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model , CancellationToken ct)
        {
            var result = await _bookingService.CreateNewBookingAsync(model, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking Created Successfully" : result.error;
            return RedirectToAction(nameof(GetMemberForUpComingSession) , new {id = model.SessionId});
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberForUpComingSession(int id , CancellationToken ct)
            => View(await _bookingService.GetMemberForUpComingBySessionIdAsync(id , ct));
        [HttpGet]
        public async Task<IActionResult> GetMemberForOnGoingSession(int id, CancellationToken ct)
            => View(await _bookingService.GetMemberForOnGoingBySessionIdAsync(id, ct));

        [HttpPost]
        public async Task<IActionResult> Attended(int memberId , int sessionId , CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendedAsync(memberId , sessionId , ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Attendance recorded" : result.error;
            return RedirectToAction(nameof(GetMemberForOnGoingSession) , new {id = sessionId});
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success
                ? "Booking cancelled successfully."
                : result.error;

            return RedirectToAction(nameof(GetMemberForUpComingSession), new { id = sessionId });
        }
    }
}
