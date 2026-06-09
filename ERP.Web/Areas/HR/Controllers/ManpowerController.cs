using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.HR.Controllers;

[Area("HR")]
public class ManpowerController : Controller
{
    private readonly IManpowerRequirementService _manpowerService;
    private readonly IEmployeeService _employeeService;
    private readonly IExcelExportService _excelExport;
    private readonly ICurrentUserService _currentUser;

    public ManpowerController(
        IManpowerRequirementService manpowerService,
        IEmployeeService employeeService,
        IExcelExportService excelExport,
        ICurrentUserService currentUser)
    {
        _manpowerService = manpowerService;
        _employeeService = employeeService;
        _excelExport = excelExport;
        _currentUser = currentUser;
    }

    [RequirePermission("HR.Manpower.View")]
    public async Task<IActionResult> StaffingPlan(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        ViewBag.Year = year;
        ViewBag.Month = month;
        ViewBag.Lookups = await _employeeService.GetFormLookupsAsync();
        return View(await _manpowerService.GetStaffingPlansAsync(year, month));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Manpower.Manage")]
    public async Task<IActionResult> SaveStaffingPlan(StaffingPlanVm model)
    {
        var result = await _manpowerService.SaveStaffingPlanAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Staffing plan saved." : result.Error;
        return RedirectToAction(nameof(StaffingPlan), new { year = model.Year, month = model.Month });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Manpower.Manage")]
    public async Task<IActionResult> DeleteStaffingPlan(int id, int year, int month)
    {
        var result = await _manpowerService.DeleteStaffingPlanAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Staffing plan deleted." : result.Error;
        return RedirectToAction(nameof(StaffingPlan), new { year, month });
    }

    [RequirePermission("HR.HiringRequest.View")]
    public async Task<IActionResult> HiringRequests(HiringRequestStatus? status)
    {
        ViewBag.Status = status;
        ViewBag.Lookups = await _employeeService.GetFormLookupsAsync();
        return View(await _manpowerService.GetHiringRequestsAsync(status));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.HiringRequest.Manage")]
    public async Task<IActionResult> SaveHiringRequest(HiringRequestVm model, bool submit)
    {
        var result = await _manpowerService.SaveHiringRequestAsync(model, submit);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? (submit ? "Hiring request submitted." : "Hiring request saved as draft.")
            : result.Error;
        return RedirectToAction(nameof(HiringRequests));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.HiringRequest.Approve")]
    public async Task<IActionResult> ApproveHiringRequest(int id, bool approved)
    {
        var result = await _manpowerService.ApproveHiringRequestAsync(id, approved);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? (approved ? "Request approved." : "Request rejected.")
            : result.Error;
        return RedirectToAction(nameof(HiringRequests));
    }

    [RequirePermission("HR.Manpower.View")]
    public async Task<IActionResult> VacancyReport(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        ViewBag.Year = year;
        ViewBag.Month = month;
        return View(await _manpowerService.GetVacancyReportAsync(year, month));
    }

    [RequirePermission("HR.Manpower.View")]
    public async Task<IActionResult> ExportVacancyReport(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        var bytes = await _excelExport.ExportVacancyReportAsync(_currentUser.CompanyId, year, month);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"VacancyReport_{year}_{month:D2}.xlsx");
    }

    [RequirePermission("HR.HiringRequest.View")]
    public async Task<IActionResult> ExportHiringRequests(HiringRequestStatus? status)
    {
        var bytes = await _excelExport.ExportHiringRequestsAsync(_currentUser.CompanyId, status);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HiringRequests.xlsx");
    }
}
