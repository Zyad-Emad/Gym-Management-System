using GymManagement.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
        private readonly GymDbContext _dbContext;
        public PlansController()
        {
            _dbContext = new GymDbContext();
        }
        //Index Action
        // GET BaseUrl/Plans/Index -> Listing ALl Plans
        public async Task<IActionResult> Index()
        {
            var plans = await _dbContext.Plans.ToListAsync();
            return View(plans);
        }
        //Details Action
        // Get BaseUrl/Plans/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _dbContext.Plans.FindAsync(id);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }
    }
}
