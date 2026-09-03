using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using ReceptionSystem.Services;

namespace ReceptionSystem.Authorization
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permission)
            : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly PermissionService _permissionService;
        private readonly string _permission;

        public PermissionFilter(
            PermissionService permissionService,
            string permission)
        {
            _permissionService = permissionService;
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    null);

                return;
            }

            var userId =
                context.HttpContext.User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission =
                await _permissionService.HasPermissionAsync(
                    userId,
                    _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}