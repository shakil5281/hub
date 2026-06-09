using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.HR.Controllers;

[Area("HR")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IExcelExportService _excelExport;
    private readonly IExcelImportService _excelImport;
    private readonly IPdfExportService _pdfExport;
    private readonly ICurrentUserService _currentUser;

    public EmployeeController(IEmployeeService employeeService, IExcelExportService excelExport, IExcelImportService excelImport, IPdfExportService pdfExport, ICurrentUserService currentUser)
    {
        _employeeService = employeeService;
        _excelExport = excelExport;
        _excelImport = excelImport;
        _pdfExport = pdfExport;
        _currentUser = currentUser;
    }

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Index([FromQuery] Core.DTOs.EmployeeFilterDto filter)
    {
        ViewBag.Filter = filter;
        ViewBag.FilterAction = Url.Action(nameof(Index));
        ViewBag.Lookups = await _employeeService.GetFormLookupsAsync();
        var employees = await _employeeService.SearchAsync(filter);
        return View(employees);
    }

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Export()
    {
        var bytes = await _excelExport.ExportEmployeesAsync(_currentUser.CompanyId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
    }

    [RequirePermission("HR.Employee.Create")]
    public IActionResult Import() => View();

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Create")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0) { TempData["Error"] = "Select a file."; return View(); }
        using var stream = file.OpenReadStream();
        var result = await _excelImport.ImportEmployeesAsync(stream, _currentUser.CompanyId, _currentUser.UserId ?? "system");
        TempData["Success"] = $"Imported {result.Imported} employees.";
        if (result.Errors.Any()) TempData["Error"] = string.Join("; ", result.Errors.Take(5));
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetDetailsAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> ProfilePdf(int id)
    {
        var bytes = await _pdfExport.ExportEmployeeProfileAsync(id, _currentUser.CompanyId);
        return File(bytes, "application/pdf", $"Employee_{id}.pdf");
    }

    [RequirePermission("HR.Employee.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Lookups = await _employeeService.GetFormLookupsAsync();
        return View(new EmployeeCreateVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Create")]
    public async Task<IActionResult> Create(EmployeeCreateVm model)
    {
        if (!ModelState.IsValid) { ViewBag.Lookups = await _employeeService.GetFormLookupsAsync(); ViewBag.ActiveTab = Request.Query["tab"].FirstOrDefault() ?? "personal"; return View(model); }
        var result = await _employeeService.CreateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Lookups = await _employeeService.GetFormLookupsAsync(); ViewBag.ActiveTab = Request.Query["tab"].FirstOrDefault() ?? "personal"; return View(model); }
        TempData["Success"] = "Employee created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _employeeService.GetForEditAsync(id);
        if (model == null) return NotFound();
        ViewBag.Lookups = await _employeeService.GetFormLookupsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Edit(EmployeeEditVm model)
    {
        if (!ModelState.IsValid) { ViewBag.Lookups = await _employeeService.GetFormLookupsAsync(); ViewBag.ActiveTab = Request.Query["tab"].FirstOrDefault() ?? "personal"; return View(model); }
        var result = await _employeeService.UpdateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Lookups = await _employeeService.GetFormLookupsAsync(); ViewBag.ActiveTab = Request.Query["tab"].FirstOrDefault() ?? "personal"; return View(model); }
        TempData["Success"] = "Employee updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Employee deleted successfully." : result.Error ?? "Unable to delete employee.";
        return RedirectToAction(nameof(Index));
    }
}
