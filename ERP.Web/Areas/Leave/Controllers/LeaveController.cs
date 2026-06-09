using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Leave.Controllers;

[Area("Leave")]
public class LeaveController : Controller
{
    private readonly ILeaveService _leaveService;
    private readonly IEmployeeService _employeeService;
    private readonly IExcelExportService _excelExport;
    private readonly ICurrentUserService _currentUser;

    public LeaveController(ILeaveService leaveService, IEmployeeService employeeService, IExcelExportService excelExport, ICurrentUserService currentUser)
    {
        _leaveService = leaveService;
        _employeeService = employeeService;
        _excelExport = excelExport;
        _currentUser = currentUser;
    }

    [RequirePermission("Leave.Manage")]
    public async Task<IActionResult> Types() => View(await _leaveService.GetLeaveTypesAsync());

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Manage")]
    public async Task<IActionResult> SaveType(LeaveTypeVm model)
    {
        var result = await _leaveService.SaveLeaveTypeAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Saved." : result.Error;
        return RedirectToAction(nameof(Types));
    }

    [RequirePermission("Leave.Manage")]
    public async Task<IActionResult> Balances(int? employeeId)
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        return View(await _leaveService.GetBalancesAsync(employeeId));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Manage")]
    public async Task<IActionResult> AdjustBalance(LeaveBalanceVm model)
    {
        var result = await _leaveService.AdjustBalanceAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Balance updated." : result.Error;
        return RedirectToAction(nameof(Balances));
    }

    [RequirePermission("Leave.Manage")]
    public async Task<IActionResult> Applications()
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        ViewBag.LeaveTypes = await _leaveService.GetLeaveTypesAsync();
        return View(await _leaveService.GetApplicationsAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Manage")]
    public async Task<IActionResult> SubmitApplication(LeaveApplicationCreateVm model)
    {
        var result = await _leaveService.SubmitApplicationAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Application submitted." : result.Error;
        return RedirectToAction(nameof(Applications));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Approve")]
    public async Task<IActionResult> Approve(int id, bool approved)
    {
        var result = await _leaveService.ApproveAsync(id, approved);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? (approved ? "Approved." : "Rejected.") : result.Error;
        return RedirectToAction(nameof(Applications));
    }

    [RequirePermission("Leave.Holiday.Manage")]
    public async Task<IActionResult> Holidays(int year = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        ViewBag.Year = year;
        return View(await _leaveService.GetHolidaysAsync(year));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Holiday.Manage")]
    public async Task<IActionResult> SaveHoliday(HolidayCreateVm model)
    {
        var result = await _leaveService.SaveHolidayAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Holiday saved." : result.Error;
        return RedirectToAction(nameof(Holidays), new { year = model.HolidayDate.Year });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Leave.Holiday.Manage")]
    public async Task<IActionResult> DeleteHoliday(int id, int year)
    {
        var result = await _leaveService.DeleteHolidayAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Deleted." : result.Error;
        return RedirectToAction(nameof(Holidays), new { year });
    }

    [RequirePermission("Leave.MonthlyRecord.View")]
    public async Task<IActionResult> MonthlyRecord(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        ViewBag.Year = year;
        ViewBag.Month = month;
        return View(await _leaveService.GetMonthlyLeaveRecordAsync(year, month));
    }

    [RequirePermission("Leave.EarnLeave.View")]
    public async Task<IActionResult> EarnLeave(int year = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        ViewBag.Year = year;
        return View(await _leaveService.GetEarnLeaveReportAsync(year));
    }

    [RequirePermission("Leave.Holiday.Manage")]
    public async Task<IActionResult> ExportHolidays(int year = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        var bytes = await _excelExport.ExportHolidaysAsync(_currentUser.CompanyId, year);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Holidays_{year}.xlsx");
    }

    [RequirePermission("Leave.MonthlyRecord.View")]
    public async Task<IActionResult> ExportMonthlyLeaveRecord(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        var bytes = await _excelExport.ExportMonthlyLeaveRecordAsync(_currentUser.CompanyId, year, month);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MonthlyLeave_{year}_{month:D2}.xlsx");
    }

    [RequirePermission("Leave.EarnLeave.View")]
    public async Task<IActionResult> ExportEarnLeave(int year = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        var bytes = await _excelExport.ExportEarnLeaveAsync(_currentUser.CompanyId, year);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EarnLeave_{year}.xlsx");
    }
}
