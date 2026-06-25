using GymManagement.BLL.ViewModels.AccountViewModels;
using GymManagement.Controllers;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager ,
            SignInManager<ApplicationUser> signInManager ,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        // GET Login -> Show Form 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // POST Login -> Submit Form 
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model , CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password");
                return View(model);
            }
            var res = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (res.Succeeded)
            {
                logger.LogInformation($"User {user.UserName} Is Signed In");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (res.IsLockedOut)
            {
                logger.LogWarning($"User {user.UserName} Is Locked Out");
                ModelState.AddModelError("InvalidLogin", "This Account Is Locked , Try Again Later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password");
                return View(model);
            }

        }
        // POST Logout 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        // GET AccessDenied 
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
