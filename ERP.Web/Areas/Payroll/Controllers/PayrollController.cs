using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Payroll.Controllers;

[Area("Payroll")]
public class PayrollController : Controller
{
    private readonly IPayrollService _payrollService;
    private readonly IEmployeeService _employeeService;
    private readonly IPdfExportService _pdfExport;
    private readonly IExcelExportService _excelExport;
    private readonly ICurrentUserService _currentUser;
    private readonly IEidBonusService _eidBonusService;

    public PayrollController(IPayrollService payrollService, IEmployeeService employeeService, IPdfExportService pdfExport, IExcelExportService excelExport, ICurrentUserService currentUser, IEidBonusService eidBonusService)
    {
        _payrollService = payrollService;
        _employeeService = employeeService;
        _pdfExport = pdfExport;
        _excelExport = excelExport;
        _currentUser = currentUser;
        _eidBonusService = eidBonusService;
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> Periods() => View(await _payrollService.GetPeriodsAsync());

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.Generate")]
    public async Task<IActionResult> Generate(int periodId)
    {
        var result = await _payrollService.GeneratePayrollAsync(periodId);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Payroll generated." : result.Error;
        return result.SheetId.HasValue ? RedirectToAction(nameof(Sheet), new { id = result.SheetId }) : RedirectToAction(nameof(Periods));
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> Sheet(int id) => View(await _payrollService.GetSheetDetailsAsync(id));

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> Summary(int sheetId) => View(await _payrollService.GetSummaryAsync(sheetId));

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> Payslip(int detailId)
    {
        var model = await _payrollService.GetPayslipAsync(detailId);
        return model == null ? NotFound() : View(model);
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> PayslipPdf(int detailId)
    {
        var bytes = await _pdfExport.ExportPayslipAsync(detailId, _currentUser.CompanyId);
        return File(bytes, "application/pdf", $"Payslip_{detailId}.pdf");
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> ExportSheet(int sheetId)
    {
        var bytes = await _excelExport.ExportMonthlyPayrollAsync(_currentUser.CompanyId, sheetId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PayrollSheet.xlsx");
    }

    [RequirePermission("Payroll.Advance.Manage")]
    public async Task<IActionResult> Advances()
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        return View(await _payrollService.GetAdvancesAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.Advance.Manage")]
    public async Task<IActionResult> SaveAdvance(AdvanceSalaryVm model)
    {
        var result = await _payrollService.SaveAdvanceAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Advance saved." : result.Error;
        return RedirectToAction(nameof(Advances));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.Advance.Manage")]
    public async Task<IActionResult> ApproveAdvance(int id)
    {
        var result = await _payrollService.ApproveAdvanceAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Advance approved." : result.Error;
        return RedirectToAction(nameof(Advances));
    }

    [RequirePermission("Payroll.Increment.Manage")]
    public async Task<IActionResult> Increments()
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        return View(await _payrollService.GetIncrementsAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.Increment.Manage")]
    public async Task<IActionResult> SaveIncrement(SalaryIncrementVm model)
    {
        var result = await _payrollService.SaveIncrementAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Increment saved." : result.Error;
        return RedirectToAction(nameof(Increments));
    }

    [RequirePermission("Payroll.Bill.Config")]
    public async Task<IActionResult> BillRates()
    {
        ViewBag.Shifts = await _payrollService.GetBillRatesAsync();
        return View(await _payrollService.GetBillRatesAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.Bill.Config")]
    public async Task<IActionResult> SaveBillRate(BillRateConfigVm model)
    {
        var result = await _payrollService.SaveBillRateAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Rate saved." : result.Error;
        return RedirectToAction(nameof(BillRates));
    }

    [RequirePermission("Payroll.DailySalary.View")]
    public async Task<IActionResult> DailySalarySheet(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        ViewBag.Date = d;
        return View(await _payrollService.GetDailySalarySheetAsync(d));
    }

    [RequirePermission("Payroll.DailySalary.View")]
    public async Task<IActionResult> DailySalarySummary(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-30);
        var t = to ?? DateTime.Today;
        ViewBag.From = f;
        ViewBag.To = t;
        return View(await _payrollService.GetDailySalarySummaryAsync(f, t));
    }

    [RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> EidBonuses(int year = 0, int bonusType = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        ViewBag.Year = year;
        ViewBag.BonusType = bonusType;
        ViewBag.Employees = await _employeeService.GetAllAsync();
        return View(await _eidBonusService.GetByYearAsync(year, bonusType));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> SaveEidBonus(EidBonusVm model)
    {
        var result = await _eidBonusService.SaveAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Bonus saved." : result.Error;
        return RedirectToAction(nameof(EidBonuses), new { year = model.Year, bonusType = model.BonusType });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> GenerateEidBonus(int year, int bonusType, decimal percentage)
    {
        var result = await _eidBonusService.GenerateBulkAsync(year, bonusType, percentage);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? $"Bonuses generated." : result.Error;
        return RedirectToAction(nameof(EidBonuses), new { year, bonusType });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> ApproveEidBonus(int id, int year, int bonusType)
    {
        var result = await _eidBonusService.ApproveAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Bonus approved." : result.Error;
        return RedirectToAction(nameof(EidBonuses), new { year, bonusType });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> DeleteEidBonus(int id, int year, int bonusType)
    {
        var result = await _eidBonusService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Deleted." : result.Error;
        return RedirectToAction(nameof(EidBonuses), new { year, bonusType });
    }

    [RequirePermission("Payroll.DailySalary.View")]
    public async Task<IActionResult> ExportDailySalarySheet(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        var bytes = await _excelExport.ExportDailySalarySheetAsync(_currentUser.CompanyId, d);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DailySalary_{d:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> ExportSummary(int sheetId)
    {
        var bytes = await _excelExport.ExportPayrollSummaryAsync(_currentUser.CompanyId, sheetId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PayrollSummary_{sheetId}.xlsx");
    }

    [RequirePermission("Payroll.DailySalary.View")]
    public async Task<IActionResult> ExportDailySalarySummary(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-30);
        var t = to ?? DateTime.Today;
        var bytes = await _excelExport.ExportDailySalarySummaryAsync(_currentUser.CompanyId, f, t);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DailySalarySummary_{f:yyyyMMdd}_{t:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Payroll.EidBonus.Manage")]
    public async Task<IActionResult> ExportEidBonus(int year, int bonusType)
    {
        var bytes = await _excelExport.ExportEidBonusAsync(_currentUser.CompanyId, year, bonusType);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EidBonus_{year}_{bonusType}.xlsx");
    }

    [RequirePermission("Payroll.View")]
    public async Task<IActionResult> ExportPayrollSheetPdf(int sheetId)
    {
        var bytes = await _pdfExport.ExportMonthlySalarySheetAsync(sheetId, _currentUser.CompanyId);
        return File(bytes, "application/pdf", $"PayrollSheet_{sheetId}.pdf");
    }
}
