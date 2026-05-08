using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sicoain.shared.Entities;

namespace sicoain.api.Data
{
    class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /* ========== CORE BUSINESS ENTITIES ========== */
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Accident> Accidents { get; set; }
        public DbSet<Witness> Witnesses { get; set; }
        public DbSet<CorrectiveAction> CorrectiveActions { get; set; }
        public DbSet<CorrectiveActionTracking> CorrectiveActionTrackings { get; set; }


        /* ========== FILES AND EVIDENCE ========== */
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<DigitalEvidence> DigitalEvidences { get; set; }


        /* ========== COMPANY ORGANIZATIONAL STRUCTURE ========== */
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }


        /* ========== CATALOGS AND CLASSIFICATIONS ========== */
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<AccidentType> AccidentTypes { get; set; }
        public DbSet<RiskClass> RiskClasses { get; set; }


        /* ========== EXTERNAL ENTITIES ========== */
        public DbSet<OccupationalRiskAdministrator> OccupationalRiskAdministrators { get; set; }
        public DbSet<HealthPromotionEntity> HealthPromotionEntities { get; set; }


        /* ========== CUSTOM ROLES AND PERMISSIONS ========== */
        public DbSet<Roles> CustomRoles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }


        /* ========== PHONES AND EMAILS - NORMALIZED TABLES ========== */
        public DbSet<BusinessEmail> BusinessEmails { get; set; }
        public DbSet<BusinessPhone> BusinessPhones { get; set; }
        public DbSet<BranchEmail> BranchEmails { get; set; }
        public DbSet<BranchPhone> BranchPhones { get; set; }
        public DbSet<EmployeeEmail> EmployeeEmails { get; set; }
        public DbSet<EmployeePhone> EmployeePhones { get; set; }
        public DbSet<OccupationalRiskAdministratorEmail> OccupationalRiskAdministratorEmails { get; set; }
        public DbSet<OccupationalRiskAdministratorPhone> OccupationalRiskAdministratorPhones { get; set; }
        public DbSet<HealthPromotionEntityEmail> HealthPromotionEntityEmails { get; set; }
        public DbSet<HealthPromotionEntityPhone> HealthPromotionEntityPhones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigureIdentityTables(builder);
            ConfigureAccidentRelationships(builder);
            ConfigureRelationships(builder);
        }

        private void ConfigureIdentityTables(ModelBuilder builder)
        {
            /* Rename Identity tables (Without "AspNet" Prefix) */
            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        }

        private void ConfigureAccidentRelationships(ModelBuilder builder)
        {
            builder.Entity<Accident>()
               .HasOne(a => a.Employee)
               .WithMany(e => e.Accidents)
               .HasForeignKey(a => a.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Accident>()
                .HasOne(a => a.AccidentType)
                .WithMany()
                .HasForeignKey(a => a.AccidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Accident>()
                .HasOne(a => a.EventCategory)
                .WithMany()
                .HasForeignKey(a => a.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureRelationships(ModelBuilder builder)
        {
            builder.Entity<Witness>()
                .HasOne(w => w.Accident)
                .WithMany(a => a.Witnesses)
                .HasForeignKey(w => w.AccidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Witness>()
                .HasOne(w => w.Employee)
                .WithMany(e => e.Witnesses)
                .HasForeignKey(w => w.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CorrectiveAction>()
                .HasOne(c => c.Accident)
                .WithMany(a => a.CorrectiveActions)
                .HasForeignKey(c => c.AccidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CorrectiveActionTracking>()
                .HasOne(ct => ct.CorrectiveAction)
                .WithMany(c => c.Trackings)
                .HasForeignKey(ct => ct.CorrectiveActionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DigitalEvidence>()
                .HasOne(de => de.Accident)
                .WithMany(a => a.DigitalEvidences)
                .HasForeignKey(de => de.AccidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Employee>()
                .HasOne(e => e.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Position>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Positions)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Branch>()
                .HasOne(b => b.Business)
                .WithMany(bus => bus.Branches)
                .HasForeignKey(b => b.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
