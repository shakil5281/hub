using ERP.Web.Core.Entities;
using ERP.Web.Core.Entities.Security;
using ERP.Web.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Data;

public static partial class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();

        var company = await context.Companies.FirstOrDefaultAsync(c => c.Code == "DEFAULT");
        if (company == null)
        {
            company = new Company
            {
                Name = "Default Company",
                Code = "DEFAULT",
                Address = "Head Office",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                Status = EntityStatus.Active
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }

        await SeedLookupsAsync(context, company.Id);
        await SeedOrganogramAsync(context, company.Id);
        await SeedPermissionsAsync(context);
        await SeedRolesAndAdminAsync(context, userManager, roleManager, company.Id);
    }

    private static async Task SeedLookupsAsync(AppDbContext context, int companyId)
    {
        if (!await context.Departments.AnyAsync(d => d.CompanyId == companyId))
        {
            context.Departments.AddRange(
                new Department { CompanyId = companyId, Name = "Human Resources", Code = "HR", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Department { CompanyId = companyId, Name = "Finance", Code = "FIN", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Department { CompanyId = companyId, Name = "Operations", Code = "OPS", CreatedAt = DateTime.UtcNow, CreatedBy = "System" });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedOrganogramAsync(AppDbContext context, int companyId)
    {
        if (await context.Sections.AnyAsync(s => s.CompanyId == companyId)) return;

        var hrDept = await context.Departments.FirstAsync(d => d.CompanyId == companyId && d.Code == "HR");
        var opsDept = await context.Departments.FirstAsync(d => d.CompanyId == companyId && d.Code == "OPS");

        var hrSection = new Section { CompanyId = companyId, DepartmentId = hrDept.Id, Name = "Admin Section", Code = "HR-ADM", CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var prodSection = new Section { CompanyId = companyId, DepartmentId = opsDept.Id, Name = "Production Section", Code = "OPS-PRD", CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        context.Sections.AddRange(hrSection, prodSection);
        await context.SaveChangesAsync();

        if (!await context.Designations.AnyAsync(d => d.CompanyId == companyId))
        {
            context.Designations.AddRange(
                new Designation { CompanyId = companyId, SectionId = hrSection.Id, Title = "Manager", Code = "MGR", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Designation { CompanyId = companyId, SectionId = hrSection.Id, Title = "Executive", Code = "EXE", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Designation { CompanyId = companyId, SectionId = prodSection.Id, Title = "Officer", Code = "OFF", CreatedAt = DateTime.UtcNow, CreatedBy = "System" });
        }

        context.Lines.AddRange(
            new Line { CompanyId = companyId, SectionId = hrSection.Id, Name = "Line A", Code = "HR-A", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            new Line { CompanyId = companyId, SectionId = prodSection.Id, Name = "Line 1", Code = "PRD-1", CreatedAt = DateTime.UtcNow, CreatedBy = "System" });
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        foreach (var code in PermissionCodes)
        {
            if (await context.Permissions.AnyAsync(p => p.Code == code)) continue;

            var module = code.Split('.')[0];
            context.Permissions.Add(new Permission
            {
                Code = code,
                Name = code.Replace('.', ' '),
                Module = module
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAndAdminAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        int companyId)
    {
        const string adminRoleName = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new ApplicationRole
            {
                Name = adminRoleName,
                Description = "System administrator with full access"
            });
        }

        var adminRole = await roleManager.FindByNameAsync(adminRoleName);
        if (adminRole != null)
        {
            var permissions = await context.Permissions.ToListAsync();
            foreach (var permission in permissions)
            {
                var exists = await context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id);
                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        const string adminEmail = "admin@erp.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Admin",
                CompanyId = companyId
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
        }
    }
}
