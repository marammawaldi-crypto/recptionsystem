using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
using ReceptionSystem.Models;

namespace ReceptionSystem.Services
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context =
                serviceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // =====================================================
            // Permissions
            // =====================================================

            var permissions = new[]
            {
                new Permission
                {
                    Name = "Visitors.View",
                    Controller = "Visitors",
                    Action = "Index"
                },

                new Permission
                {
                    Name = "Visitors.Details",
                    Controller = "Visitors",
                    Action = "Details"
                },

                new Permission
                {
                    Name = "Visitors.Create",
                    Controller = "Visitors",
                    Action = "Create"
                },

                new Permission
                {
                    Name = "Visitors.Edit",
                    Controller = "Visitors",
                    Action = "Edit"
                },

                new Permission
                {
                    Name = "JobApplications.View",
                    Controller = "JobApplications",
                    Action = "Index"
                },

                new Permission
                {
                    Name = "JobApplications.Create",
                    Controller = "JobApplications",
                    Action = "Create"
                },

                new Permission
                {
                    Name = "JobApplications.Edit",
                    Controller = "JobApplications",
                    Action = "Edit"
                },

                new Permission
                {
                    Name = "JobApplications.Details",
                    Controller = "JobApplications",
                    Action = "Details"
                },

                new Permission
                {
                    Name = "JobApplications.DownloadCv",
                    Controller = "JobApplications",
                    Action = "DownloadCv"
                },

                new Permission
                {
                    Name = "JobApplications.Export",
                    Controller = "JobApplications",
                    Action = "ExportToExcel"
                },

                new Permission
                {
                    Name = "JobApplications.Print",
                    Controller = "JobApplications",
                    Action = "Print"
                },

                new Permission
                {
                    Name = "Visitors.Delete",
                    Controller = "Visitors",
                    Action = "Delete"
                },

                new Permission
                {
                    Name = "JobApplications.Delete",
                    Controller = "JobApplications",
                    Action = "Delete"
                }
            };


            // =====================================================
            // Create Permissions if they don't exist
            // =====================================================

            foreach (var permission in permissions)
            {
                var existingPermission =
                    await context.Permissions
                        .FirstOrDefaultAsync(p =>
                            p.Name == permission.Name);

                if (existingPermission == null)
                {
                    context.Permissions.Add(permission);
                }
            }

            await context.SaveChangesAsync();


            // =====================================================
            // Roles
            // =====================================================

            var adminRole =
                await roleManager.FindByNameAsync("ADMIN");

            var receptionRole =
                await roleManager.FindByNameAsync("RECEPTIONIST");

            var userRole =
                await roleManager.FindByNameAsync("USER");


            // =====================================================
            // Get Permissions
            // =====================================================

            var visitorsView =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "Visitors.View");

            var visitorsDetails =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "Visitors.Details");

            var visitorsCreate =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "Visitors.Create");

            var visitorsEdit =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "Visitors.Edit");

            var jobApplicationsView =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.View");

            var jobApplicationsCreate =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Create");

            var jobApplicationsEdit =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Edit");

            var jobApplicationsDetails =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Details");

            var jobApplicationsDownloadCv =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.DownloadCv");

            var jobApplicationsExport =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Export");

            var jobApplicationsPrint =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Print");

            var visitorsDelete =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "Visitors.Delete");

            var jobApplicationsDelete =
                await context.Permissions
                    .FirstAsync(p =>
                        p.Name == "JobApplications.Delete");


            // =====================================================
            // ADMIN Permissions
            // =====================================================

            if (adminRole != null)
            {
                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    visitorsView.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    visitorsDetails.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    visitorsCreate.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    visitorsEdit.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsView.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsCreate.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsEdit.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsDetails.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsDownloadCv.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsExport.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsPrint.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    visitorsDelete.Id);

                await AddPermissionAsync(
                    context,
                    adminRole.Id,
                    jobApplicationsDelete.Id);
            }


            // =====================================================
            // RECEPTIONIST Permissions
            // =====================================================

            if (receptionRole != null)
            {
                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    visitorsView.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    visitorsDetails.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    visitorsCreate.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    visitorsEdit.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsView.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsEdit.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsDetails.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsDownloadCv.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsExport.Id);

                await AddPermissionAsync(
                    context,
                    receptionRole.Id,
                    jobApplicationsPrint.Id);
            }


            // =====================================================
            // USER Permissions
            // =====================================================

            if (userRole != null)
            {
                // USER يستطيع إنشاء طلب توظيف فقط
                await AddPermissionAsync(
                    context,
                    userRole.Id,
                    jobApplicationsCreate.Id);


                // -------------------------------------------------
                // إزالة صلاحية عرض الزوار القديمة من USER
                // -------------------------------------------------

                var oldVisitorsPermission =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == visitorsView.Id);

                if (oldVisitorsPermission != null)
                {
                    context.RolePermissions.Remove(
                        oldVisitorsPermission);
                }


                // -------------------------------------------------
                // إزالة صلاحية تفاصيل الزوار من USER
                // -------------------------------------------------

                var oldVisitorsDetails =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == visitorsDetails.Id);

                if (oldVisitorsDetails != null)
                {
                    context.RolePermissions.Remove(
                        oldVisitorsDetails);
                }


                // -------------------------------------------------
                // إزالة صلاحية إنشاء الزوار من USER
                // -------------------------------------------------

                var oldVisitorsCreate =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == visitorsCreate.Id);

                if (oldVisitorsCreate != null)
                {
                    context.RolePermissions.Remove(
                        oldVisitorsCreate);
                }


                // -------------------------------------------------
                // إزالة صلاحية تعديل الزوار من USER
                // -------------------------------------------------

                var oldVisitorsEdit =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == visitorsEdit.Id);

                if (oldVisitorsEdit != null)
                {
                    context.RolePermissions.Remove(
                        oldVisitorsEdit);
                }


                // -------------------------------------------------
                // إزالة صلاحية عرض طلبات التوظيف القديمة من USER
                // -------------------------------------------------

                var oldJobApplicationsView =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsView.Id);

                if (oldJobApplicationsView != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsView);
                }


                // -------------------------------------------------
                // إزالة صلاحية تعديل طلبات التوظيف من USER
                // -------------------------------------------------

                var oldJobApplicationsEdit =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsEdit.Id);

                if (oldJobApplicationsEdit != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsEdit);
                }


                // -------------------------------------------------
                // إزالة صلاحية تفاصيل طلبات التوظيف من USER
                // -------------------------------------------------

                var oldJobApplicationsDetails =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsDetails.Id);

                if (oldJobApplicationsDetails != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsDetails);
                }


                // -------------------------------------------------
                // إزالة صلاحية تحميل السيرة الذاتية من USER
                // -------------------------------------------------

                var oldJobApplicationsDownloadCv =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsDownloadCv.Id);

                if (oldJobApplicationsDownloadCv != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsDownloadCv);
                }


                // -------------------------------------------------
                // إزالة صلاحية التصدير من USER
                // -------------------------------------------------

                var oldJobApplicationsExport =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsExport.Id);

                if (oldJobApplicationsExport != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsExport);
                }


                // -------------------------------------------------
                // إزالة صلاحية الطباعة من USER
                // -------------------------------------------------

                var oldJobApplicationsPrint =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsPrint.Id);

                if (oldJobApplicationsPrint != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsPrint);
                }


                // -------------------------------------------------
                // إزالة صلاحية حذف الزوار من USER
                // -------------------------------------------------

                var oldVisitorsDelete =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == visitorsDelete.Id);

                if (oldVisitorsDelete != null)
                {
                    context.RolePermissions.Remove(
                        oldVisitorsDelete);
                }


                // -------------------------------------------------
                // إزالة صلاحية حذف طلبات التوظيف من USER
                // -------------------------------------------------

                var oldJobApplicationsDelete =
                    await context.RolePermissions
                        .FirstOrDefaultAsync(rp =>
                            rp.RoleId == userRole.Id &&
                            rp.PermissionId == jobApplicationsDelete.Id);

                if (oldJobApplicationsDelete != null)
                {
                    context.RolePermissions.Remove(
                        oldJobApplicationsDelete);
                }
            }


            // =====================================================
            // Save Changes
            // =====================================================

            await context.SaveChangesAsync();
        }


        // =====================================================
        // Add Permission to Role
        // =====================================================

        private static async Task AddPermissionAsync(
            ApplicationDbContext context,
            string roleId,
            int permissionId)
        {
            var exists =
                await context.RolePermissions
                    .AnyAsync(rp =>
                        rp.RoleId == roleId &&
                        rp.PermissionId == permissionId);

            if (!exists)
            {
                context.RolePermissions.Add(
                    new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    });
            }
        }
    }
}