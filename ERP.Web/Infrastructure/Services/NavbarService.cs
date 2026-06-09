using System.Security.Claims;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using ERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class NavbarService : INavbarService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly IUrlHelperFactory _urlHelperFactory;

    public NavbarService(
        AppDbContext context,
        ICurrentUserService currentUser,
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator,
        IUrlHelperFactory urlHelperFactory)
    {
        _context = context;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
        _urlHelperFactory = urlHelperFactory;
    }

    public async Task<GlobalSearchResponseVm> SearchAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return new GlobalSearchResponseVm();

        var term = query.Trim().ToLowerInvariant();
        var companyId = _currentUser.CompanyId;
        var groups = new List<GlobalSearchGroupVm>();

        if (await HasPermissionAsync("HR.Employee.View"))
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.CompanyId == companyId &&
                    (e.FullName.ToLower().Contains(term) ||
                     e.EmployeeCode.ToLower().Contains(term) ||
                     e.PunchNumber.ToLower().Contains(term) ||
                     (e.Email != null && e.Email.ToLower().Contains(term))))
                .OrderBy(e => e.EmployeeCode)
                .Take(8)
                .ToListAsync();

            if (employees.Count > 0)
            {
                groups.Add(new GlobalSearchGroupVm
                {
                    Category = "Employees",
                    Items = employees.Select(e => new GlobalSearchItemVm
                    {
                        Title = e.FullName,
                        Subtitle = $"{e.EmployeeCode} · {e.Department?.Name ?? "Unassigned"}",
                        Href = EnsurePath(Url("Details", "Employee", new { area = "HR", id = e.Id })),
                        Icon = "briefcase"
                    }).ToList()
                });
            }
        }

        var words = term.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var pages = GetPageLinks()
            .Where(l => words.Length == 0 || words.All(w => l.Title.Contains(w, StringComparison.OrdinalIgnoreCase)))
            .Take(12)
            .ToList();

        if (pages.Count > 0)
            groups.Add(new GlobalSearchGroupVm { Category = "Pages", Items = pages });

        return new GlobalSearchResponseVm { Groups = groups };
    }

    public Task<IReadOnlyList<GlobalSearchItemVm>> GetQuickLinksAsync()
    {
        var links = GetPageLinks().Take(6).ToList();
        return Task.FromResult<IReadOnlyList<GlobalSearchItemVm>>(links);
    }

    private IReadOnlyList<GlobalSearchItemVm> GetPageLinks()
    {
        var url = GetUrlHelper();
        if (url == null) return Array.Empty<GlobalSearchItemVm>();

        return SidebarNavigation.GetFlatLinks(url)
            .Select(l => new GlobalSearchItemVm
            {
                Title = l.Label,
                Subtitle = "Go to page",
                Href = l.Href.StartsWith('/') ? l.Href : "/" + l.Href.TrimStart('/'),
                Icon = l.Icon
            })
            .ToList();
    }

    public async Task<NotificationsResponseVm> GetNotificationsAsync()
    {
        var companyId = _currentUser.CompanyId;
        var today = DateTime.Today;
        var items = new List<NotificationItemVm>();

        if (await HasPermissionAsync("Leave.Approve") || await HasPermissionAsync("Leave.Manage"))
        {
            var pendingLeaves = await _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Where(l => l.CompanyId == companyId && l.ApprovalStatus == LeaveApprovalStatus.Pending)
                .OrderByDescending(l => l.CreatedAt)
                .Take(8)
                .ToListAsync();

            items.AddRange(pendingLeaves.Select(l => new NotificationItemVm
            {
                Id = $"leave-{l.Id}",
                Title = "Leave approval required",
                Message = $"{l.Employee?.FullName} — {l.LeaveType?.Name} ({l.FromDate:dd MMM} – {l.ToDate:dd MMM})",
                Href = Url("Applications", "Leave", new { area = "Leave" }),
                Type = "leave",
                CreatedAt = l.CreatedAt
            }));
        }

        if (await HasPermissionAsync("Payroll.Advance.Manage"))
        {
            var pendingAdvances = await _context.AdvanceSalaries
                .Include(a => a.Employee)
                .Where(a => a.CompanyId == companyId && a.AdvanceStatus == AdvanceSalaryStatus.Pending)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            items.AddRange(pendingAdvances.Select(a => new NotificationItemVm
            {
                Id = $"advance-{a.Id}",
                Title = "Advance salary pending",
                Message = $"{a.Employee?.FullName} — {a.Amount:N2}",
                Href = Url("Advances", "Payroll", new { area = "Payroll" }),
                Type = "advance",
                CreatedAt = a.CreatedAt
            }));
        }

        if (await HasPermissionAsync("Attendance.Process"))
        {
            var pendingAttendance = await _context.DailyAttendances
                .CountAsync(a => a.CompanyId == companyId && a.AttendanceDate == today && !a.IsApproved && !a.IsAbsent);

            if (pendingAttendance > 0)
            {
                items.Add(new NotificationItemVm
                {
                    Id = "attendance-pending",
                    Title = "Attendance needs approval",
                    Message = $"{pendingAttendance} record(s) for {today:dd MMM yyyy} awaiting approval",
                    Href = Url("Daily", "Attendance", new { area = "Attendance", date = today.ToString("yyyy-MM-dd") }),
                    Type = "attendance",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (await HasPermissionAsync("Payroll.EidBonus.Manage"))
        {
            var draftBonuses = await _context.EidBonuses
                .CountAsync(b => b.CompanyId == companyId && b.BonusStatus == BonusStatus.Draft);

            if (draftBonuses > 0)
            {
                items.Add(new NotificationItemVm
                {
                    Id = "eid-bonus-draft",
                    Title = "Eid bonus drafts",
                    Message = $"{draftBonuses} bonus record(s) in draft status",
                    Href = Url("EidBonuses", "Payroll", new { area = "Payroll" }),
                    Type = "bonus",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (await HasPermissionAsync("Payroll.Generate"))
        {
            var openPeriods = await _context.PayrollPeriods
                .Where(p => p.CompanyId == companyId && !_context.PayrollSheets.Any(s => s.PayrollPeriodId == p.Id && s.CompanyId == companyId))
                .OrderByDescending(p => p.StartDate)
                .Take(3)
                .ToListAsync();

            items.AddRange(openPeriods.Select(p => new NotificationItemVm
            {
                Id = $"payroll-{p.Id}",
                Title = "Payroll not generated",
                Message = $"Period \"{p.Name}\" is ready for salary processing",
                Href = Url("Periods", "Payroll", new { area = "Payroll" }),
                Type = "payroll",
                CreatedAt = p.CreatedAt
            }));
        }

        var ordered = items.OrderByDescending(i => i.CreatedAt).Take(15).ToList();
        return new NotificationsResponseVm
        {
            UnreadCount = ordered.Count,
            Items = ordered
        };
    }

    private IUrlHelper? GetUrlHelper()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;
        var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return _urlHelperFactory.GetUrlHelper(actionContext);
    }

    private string Url(string action, string controller, object? values = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return "#";

        var href = _linkGenerator.GetPathByAction(httpContext, action, controller, values);
        return EnsurePath(href ?? "#");
    }

    private static string EnsurePath(string href)
        => href.StartsWith('/') ? href : "/" + href.TrimStart('/');

    private async Task<bool> HasPermissionAsync(string permissionCode)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return false;

        var roleNames = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (roleNames.Count == 0) return false;

        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .AnyAsync(rp =>
                roleNames.Contains(rp.Role!.Name!) &&
                rp.Permission!.Code == permissionCode);
    }
}
