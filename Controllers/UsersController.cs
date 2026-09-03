using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Models;

namespace ReceptionSystem.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // =====================================================
        // Index - Display Users
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "بدون دور"
                });
            }

            return View(userList);
        }

        // =====================================================
        // Create - GET
        // =====================================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =====================================================
        // Create - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // =================================================
            // Check Role
            // =================================================

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                ModelState.AddModelError(
                    "Role",
                    "نوع المستخدم غير موجود.");

                return View(model);
            }

            // =================================================
            // Create User
            // =================================================

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            // =================================================
            // Creation Failed
            // =================================================

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // =================================================
            // Add Role
            // =================================================

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                model.Role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                // Delete user if role assignment failed
                await _userManager.DeleteAsync(user);

                return View(model);
            }

            // =================================================
            // Success
            // =================================================

            TempData["SuccessMessage"] =
                "تم إنشاء المستخدم بنجاح.";

            return RedirectToAction("Create");
        }
    
    // =====================================================
// Edit - GET
// =====================================================

[HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? ""
            };

            return View(model);
        }

        // =====================================================
        // Edit - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check Role
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                ModelState.AddModelError(
                    "Role",
                    "نوع المستخدم غير موجود.");

                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Update username
            user.UserName = model.UserName;

            // Update email
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove old roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles);
            }

            // Add new role
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                model.Role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            TempData["SuccessMessage"] =
                "تم تعديل المستخدم بنجاح.";

            return RedirectToAction("Index");
        }
    
    // =====================================================
// Delete - POST
// =====================================================

[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Get current logged-in user
            var currentUserId = _userManager.GetUserId(User);

            // Prevent deleting yourself
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "لا يمكنك حذف المستخدم الذي سجلت الدخول به.";

                return RedirectToAction("Index");
            }

            // Find user
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Delete user
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] = error.Description;
                }

                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] =
                "تم حذف المستخدم بنجاح.";

            return RedirectToAction("Index");
        }
    }
}





























