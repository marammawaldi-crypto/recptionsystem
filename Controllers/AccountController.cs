using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReceptionSystem.Models;
using System.Threading.Tasks;

namespace ReceptionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName.Trim());
            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "المستخدم غير موجود");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // =====================================================
                // Get User Role
                // =====================================================

                var roles = await _userManager.GetRolesAsync(user);

                // =====================================================
                // Redirect by Role
                // =====================================================

                if (roles.Contains("ADMIN"))
                {
                    return RedirectToAction("Index", "Home");
                }

                if (roles.Contains("RECEPTIONIST"))
                {
                    return RedirectToAction("Index", "Visitors");
                }

                if (roles.Contains("USER"))
                {
                    return RedirectToAction("Create", "JobApplications");
                }

                // =====================================================
                // Unknown Role
                // =====================================================

                await _signInManager.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "لا يوجد دور محدد لهذا المستخدم.");

                return View(model);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "تم قفل الحساب");

                return View(model);
            }

            if (result.RequiresTwoFactor)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "يتطلب مصادقة بخطوتين");

                return View(model);
            }

            ModelState.AddModelError(
                string.Empty,
                "كلمة المرور خاطئة");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}