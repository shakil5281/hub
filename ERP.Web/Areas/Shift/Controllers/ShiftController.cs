using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Shift.Controllers;

[Area("Shift")]
public class ShiftController : Controller
{
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;
    private readonly ITemporaryShiftService _temporaryShiftService;

    public ShiftController(IShiftService shiftService, IEmployeeService employeeService, ITemporaryShiftService temporaryShiftService)
    {
        _shiftService = shiftService;
        _employeeService = employeeService;
        _temporaryShiftService = temporaryShiftService;
    }

    [RequirePermission("Shift.Manage")]
    public async Task<IActionResult> Index() => View(await _shiftService.GetAllAsync());

    [RequirePermission("Shift.Manage")]
    public IActionResult Create() => View("Edit", new ShiftEditVm { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), GraceMinutes = 10, PunchWindowBeforeMinutes = 60, MaxOvertimeMinutes = 120 });

    [RequirePermission("Shift.Manage")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _shiftService.GetForEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Shift.Manage")]
    public async Task<IActionResult> Edit(ShiftEditVm model)
    {
        var result = await _shiftService.SaveAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Shift saved." : result.Error;
        return result.Success ? RedirectToAction(nameof(Index)) : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Shift.Manage")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _shiftService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Deleted." : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Shift.Manage")]
    public async Task<IActionResult> Assignments()
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        ViewBag.Shifts = await _shiftService.GetAllAsync();
        return View(await _shiftService.GetAssignmentsAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Shift.Manage")]
    public async Task<IActionResult> SaveAssignment(ShiftAssignmentVm model)
    {
        var result = await _shiftService.SaveAssignmentAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Assignment saved." : result.Error;
        return RedirectToAction(nameof(Assignments));
    }

    [RequirePermission("Shift.Temporary.Manage")]
    public async Task<IActionResult> TemporaryAssignments(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-7);
        var t = to ?? DateTime.Today.AddDays(7);
        ViewBag.From = f;
        ViewBag.To = t;
        ViewBag.Employees = await _employeeService.GetAllAsync();
        ViewBag.Shifts = await _shiftService.GetAllAsync();
        return View(await _temporaryShiftService.GetByDateRangeAsync(f, t));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Shift.Temporary.Manage")]
    public async Task<IActionResult> SaveTemporaryAssignment(TemporaryShiftCreateVm model)
    {
        var result = await _temporaryShiftService.SaveAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Temporary assignment saved." : result.Error;
        return RedirectToAction(nameof(TemporaryAssignments));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Shift.Temporary.Manage")]
    public async Task<IActionResult> DeleteTemporaryAssignment(int id)
    {
        var result = await _temporaryShiftService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Deleted." : result.Error;
        return RedirectToAction(nameof(TemporaryAssignments));
    }
}
