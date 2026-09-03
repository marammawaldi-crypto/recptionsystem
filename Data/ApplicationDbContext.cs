using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Models;

namespace ReceptionSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<DrivingLicenseType> DrivingLicenseTypes { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<ComputerSkill> ComputerSkills { get; set; }

        // =====================================================
        // Permissions
        // =====================================================

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // RolePermission → Permission
            // =====================================================

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // Driving License Types
            // =====================================================

            modelBuilder.Entity<DrivingLicenseType>().HasData(
                new DrivingLicenseType { Id = 1, Category = "A" },
                new DrivingLicenseType { Id = 2, Category = "B" },
                new DrivingLicenseType { Id = 3, Category = "C" },
                new DrivingLicenseType { Id = 4, Category = "D" },
                new DrivingLicenseType { Id = 5, Category = "D1" },
                new DrivingLicenseType { Id = 6, Category = "D2" }
            );
        }
    }
}