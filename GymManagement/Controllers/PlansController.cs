using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
        //private readonly GymDbContext _dbContext;
        private readonly IPlanRepository planRepository;
        public PlansController(IPlanRepository repository)
        {
            planRepository = repository;
        }
        //Index Action
        // GET BaseUrl/Plans/Index -> Listing ALl Plans
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct : ct);
            return View(plans);
        }
        //Details Action
        // Get BaseUrl/Plans/Details/1
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id , ct);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }
    }
}
