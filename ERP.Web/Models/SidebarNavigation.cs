using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Models;

public class SidebarVm
{
    public string DashboardHref { get; init; } = "/";
    public string QuickCreateHref { get; init; } = "/";
    public string QuickInboxHref { get; init; } = "/";
    public bool IsDashboardActive { get; init; }
    public IReadOnlyList<SidebarGroupVm> Groups { get; init; } = Array.Empty<SidebarGroupVm>();
}

public static class SidebarNavigation
{
    public static SidebarVm Build(IUrlHelper url, string? currentPath)
    {
        currentPath = (currentPath ?? "/").TrimEnd('/');
        if (currentPath == "") currentPath = "/";

        string A(string action, string controller, object? values = null)
            => (url.Action(action, controller, values) ?? "/").TrimEnd('/');

        var dash = A("Index", "Home");
        var groups = new List<SidebarGroupVm>
        {
            new()
            {
                Title = "Administration",
                OpenByDefault = IsInArea(currentPath, "/Admin"),
                Links =
                [
                    Link("Users", A("Index", "User", new { area = "Admin" }), "users", currentPath),
                    Link("Roles", A("Index", "Role", new { area = "Admin" }), "shield", currentPath)
                ]
            },
            new()
            {
                Title = "Company",
                OpenByDefault = IsInArea(currentPath, "/Company"),
                Links =
                [
                    Link("Company Information", A("Index", "Company", new { area = "Company" }), "building", currentPath),
                    Link("Organogram", A("Index", "Organogram", new { area = "Company" }), "file", currentPath)
                ]
            },
            new()
            {
                Title = "Human Resources",
                OpenByDefault = IsInArea(currentPath, "/HR"),
                Links =
                [
                    Link("Employees", A("Index", "Employee", new { area = "HR" }), "briefcase", currentPath),
                    Link("Addresses", A("Index", "Address", new { area = "HR" }), "file", currentPath),
                    Link("Staffing Plan", A("StaffingPlan", "Manpower", new { area = "HR" }), "users", currentPath),
                    Link("Hiring Requests", A("HiringRequests", "Manpower", new { area = "HR" }), "file", currentPath),
                    Link("Vacancy Report", A("VacancyReport", "Manpower", new { area = "HR" }), "file", currentPath),
                    Link("Employee Separation", A("Index", "Separation", new { area = "HR" }), "users", currentPath),
                    Link("Add Employee", A("Create", "Employee", new { area = "HR" }), "users", currentPath),
                    Link("Import Employees", A("Import", "Employee", new { area = "HR" }), "file", currentPath)
                ]
            },
            new()
            {
                Title = "Shift Management",
                OpenByDefault = IsInArea(currentPath, "/Shift"),
                Links =
                [
                    Link("Shifts", A("Index", "Shift", new { area = "Shift" }), "clock", currentPath),
                    Link("Assignments", A("Assignments", "Shift", new { area = "Shift" }), "clock", currentPath),
                    Link("Temporary Shifts", A("TemporaryAssignments", "Shift", new { area = "Shift" }), "clock", currentPath)
                ]
            },
            new()
            {
                Title = "Attendance",
                OpenByDefault = IsInArea(currentPath, "/Attendance"),
                Links =
                [
                    Link("Punch Records", A("Index", "Punch", new { area = "Attendance" }), "calendar", currentPath),
                    Link("Import Punch", A("Import", "Punch", new { area = "Attendance" }), "file", currentPath),
                    Link("Process Attendance", A("Process", "Attendance", new { area = "Attendance" }), "clock", currentPath),
                    Link("Daily List", A("Daily", "Attendance", new { area = "Attendance" }), "calendar", currentPath),
                    Link("Attendance Report", A("Report", "Attendance", new { area = "Attendance" }), "file", currentPath),
                    Link("Attendance Summary", A("Summary", "Attendance", new { area = "Attendance" }), "file", currentPath),
                    Link("Manpower", A("Manpower", "Attendance", new { area = "Attendance" }), "users", currentPath),
                    Link("Job Card", A("JobCard", "Attendance", new { area = "Attendance" }), "file", currentPath),
                    Link("Absent Status", A("AbsentStatus", "Attendance", new { area = "Attendance" }), "calendar", currentPath),
                    Link("OT Deduction", A("OtDeduction", "Attendance", new { area = "Attendance" }), "file", currentPath),
                    Link("Daily Overtime", A("DailyOvertime", "Attendance", new { area = "Attendance" }), "clock", currentPath),
                    Link("OT Summary", A("DailyOvertimeSummary", "Attendance", new { area = "Attendance" }), "file", currentPath)
                ]
            },
            new()
            {
                Title = "Leave Management",
                OpenByDefault = IsInArea(currentPath, "/Leave"),
                Links =
                [
                    Link("Leave Types", A("Types", "Leave", new { area = "Leave" }), "calendar", currentPath),
                    Link("Leave Balances", A("Balances", "Leave", new { area = "Leave" }), "wallet", currentPath),
                    Link("Applications", A("Applications", "Leave", new { area = "Leave" }), "calendar", currentPath),
                    Link("Holidays", A("Holidays", "Leave", new { area = "Leave" }), "calendar", currentPath),
                    Link("Monthly Record", A("MonthlyRecord", "Leave", new { area = "Leave" }), "file", currentPath),
                    Link("Earn Leave", A("EarnLeave", "Leave", new { area = "Leave" }), "file", currentPath)
                ]
            },
            new()
            {
                Title = "Payroll",
                OpenByDefault = IsInArea(currentPath, "/Payroll"),
                Links =
                [
                    Link("Salary Process", A("Periods", "Payroll", new { area = "Payroll" }), "wallet", currentPath),
                    Link("Salary Advances", A("Advances", "Payroll", new { area = "Payroll" }), "wallet", currentPath),
                    Link("Increments", A("Increments", "Payroll", new { area = "Payroll" }), "wallet", currentPath),
                    Link("Bill Rates", A("BillRates", "Payroll", new { area = "Payroll" }), "file", currentPath),
                    Link("Daily Salary Sheet", A("DailySalarySheet", "Payroll", new { area = "Payroll" }), "wallet", currentPath),
                    Link("Daily Salary Summary", A("DailySalarySummary", "Payroll", new { area = "Payroll" }), "file", currentPath),
                    Link("Eid Bonus", A("EidBonuses", "Payroll", new { area = "Payroll" }), "wallet", currentPath)
                ]
            },
            new()
            {
                Title = "Reports",
                OpenByDefault = IsInArea(currentPath, "/Reports"),
                Links =
                [
                    Link("Monthly Report", A("Index", "MonthlyReport", new { area = "Reports" }), "file", currentPath)
                ]
            }
        };

        foreach (var g in groups)
            g.OpenByDefault = g.OpenByDefault || g.Links.Any(l => l.IsActive);

        return new SidebarVm
        {
            DashboardHref = dash,
            QuickCreateHref = A("Create", "Employee", new { area = "HR" }),
            QuickInboxHref = A("Applications", "Leave", new { area = "Leave" }),
            IsDashboardActive = currentPath == "/" || currentPath.Equals(dash, StringComparison.OrdinalIgnoreCase),
            Groups = groups
        };
    }

    private static SidebarLinkVm Link(string label, string href, string icon, string currentPath)
        => new() { Label = label, Href = href, Icon = icon, IsActive = IsActive(currentPath, href) };

    public static bool IsActive(string? currentPath, string href)
    {
        currentPath = (currentPath ?? "/").TrimEnd('/');
        href = href.TrimEnd('/');
        if (string.IsNullOrEmpty(href) || href == "/")
            return currentPath == "/" || currentPath == "";
        return currentPath.Equals(href, StringComparison.OrdinalIgnoreCase)
            || currentPath.StartsWith(href + "/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInArea(string currentPath, string areaPrefix)
        => currentPath.StartsWith(areaPrefix, StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<SidebarLinkVm> GetFlatLinks(IUrlHelper url, string? currentPath = null)
    {
        var vm = Build(url, currentPath);
        var links = new List<SidebarLinkVm>
        {
            new() { Label = "Dashboard", Href = vm.DashboardHref, Icon = "dashboard" }
        };
        foreach (var group in vm.Groups)
            links.AddRange(group.Links);
        return links;
    }
}
