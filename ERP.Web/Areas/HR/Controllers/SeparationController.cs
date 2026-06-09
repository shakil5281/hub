using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.HR.Controllers;

[Area("HR")]
public class SeparationController : Controller
{
    private readonly ISeparationService _separationService;
    private readonly IEmployeeService _employeeService;
    private readonly IOrganogramService _organogramService;
    private readonly IExcelExportService _excelExport;
    private readonly ICurrentUserService _currentUser;

    public SeparationController(
        ISeparationService separationService,
        IEmployeeService employeeService,
        IOrganogramService organogramService,
        IExcelExportService excelExport,
        ICurrentUserService currentUser)
    {
        _separationService = separationService;
        _employeeService = employeeService;
        _organogramService = organogramService;
        _excelExport = excelExport;
        _currentUser = currentUser;
    }

    [RequirePermission("HR.Separation.View")]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to, SeparationType? separationType, int? departmentId)
    {
        var filter = new SeparationFilterVm
        {
            From = from,
            To = to,
            SeparationType = separationType,
            DepartmentId = departmentId
        };
        ViewBag.Filter = filter;
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        return View(await _separationService.GetSeparationsAsync(filter));
    }

    [RequirePermission("HR.Separation.Manage")]
    public async Task<IActionResult> Create(int? employeeId)
    {
        var model = await _separationService.GetCreateModelAsync(employeeId) ?? new SeparationCreateVm();
        ViewBag.Employees = (await _employeeService.GetAllAsync())
            .Where(e => e.Status == EntityStatus.Active)
            .OrderBy(e => e.EmployeeCode);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Separation.Manage")]
    public async Task<IActionResult> Create(SeparationCreateVm model)
    {
        var result = await _separationService.CreateSeparationAsync(model);
        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            ViewBag.Employees = (await _employeeService.GetAllAsync())
                .Where(e => e.Status == EntityStatus.Active)
                .OrderBy(e => e.EmployeeCode);
            return View(model);
        }

        TempData["Success"] = "Employee separation recorded.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("HR.Separation.View")]
    public async Task<IActionResult> Export(DateTime? from, DateTime? to, SeparationType? separationType, int? departmentId)
    {
        var bytes = await _excelExport.ExportSeparationsAsync(_currentUser.CompanyId, from, to, separationType, departmentId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeSeparations.xlsx");
    }
}
