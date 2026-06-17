using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
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
    }
}
