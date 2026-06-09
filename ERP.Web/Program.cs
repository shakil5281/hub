using ERP.Web.Core.Entities.Security;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using ERP.Web.Infrastructure.Data;
using ERP.Web.Infrastructure.Export;
using ERP.Web.Infrastructure.Identity;
using ERP.Web.Infrastructure.Repositories;
using ERP.Web.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialOrganization("ERP");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IDailyAttendanceRepository, DailyAttendanceRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ILineRepository, LineRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IPunchRecordRepository, PunchRecordRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INavbarService, NavbarService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrganogramService, OrganogramService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IPunchRecordService, PunchRecordService>();
builder.Services.AddScoped<IPunchImportService, PunchImportService>();
builder.Services.AddScoped<IAttendanceProcessService, AttendanceProcessService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddScoped<IPdfExportService, PdfExportService>();
builder.Services.AddScoped<IFilterPresetService, FilterPresetService>();
builder.Services.AddScoped<ITemporaryShiftRepository, TemporaryShiftRepository>();
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<IEidBonusRepository, EidBonusRepository>();
builder.Services.AddScoped<IJobCardRepository, JobCardRepository>();
builder.Services.AddScoped<ITemporaryShiftService, TemporaryShiftService>();
builder.Services.AddScoped<IEidBonusService, EidBonusService>();
builder.Services.AddScoped<IJobCardService, JobCardService>();
builder.Services.AddScoped<IMonthlyReportService, MonthlyReportService>();
builder.Services.AddScoped<IStaffingPlanRepository, StaffingPlanRepository>();
builder.Services.AddScoped<IHiringRequestRepository, HiringRequestRepository>();
builder.Services.AddScoped<IEmployeeSeparationRepository, EmployeeSeparationRepository>();
builder.Services.AddScoped<IManpowerRequirementService, ManpowerRequirementService>();
builder.Services.AddScoped<ISeparationService, SeparationService>();

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in DbSeeder.PermissionCodes)
    {
        options.AddPolicy($"Permission:{permission}", policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

var pathBase = builder.Configuration["PathBase"]
    ?? builder.Configuration["ASPNETCORE_PATHBASE"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await DbSeeder.SeedAsync(app.Services);

app.Run();
