using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;

namespace ReceptionSystem.Services
{
    public class PermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PermissionService(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> HasPermissionAsync(
            string userId,
            string permissionName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            // ADMIN لديه كل الصلاحيات
            if (await _userManager.IsInRoleAsync(user, "ADMIN"))
                return true;

            var hasPermission =
                await (
                    from role in _context.Roles
                    join userRole in _context.UserRoles
                        on role.Id equals userRole.RoleId
                    join rolePermission in _context.RolePermissions
                        on role.Id equals rolePermission.RoleId
                    join permission in _context.Permissions
                        on rolePermission.PermissionId equals permission.Id
                    where userRole.UserId == userId
                          && permission.Name == permissionName
                    select permission
                ).AnyAsync();

            return hasPermission;
        }
    }
}