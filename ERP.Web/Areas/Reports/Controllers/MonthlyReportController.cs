using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Reports.Controllers;

[Area("Reports")]
public class MonthlyReportController : Controller
{
    private readonly IMonthlyReportService _reportService;
    private readonly IExcelExportService _excelExport;
    private readonly IPdfExportService _pdfExport;
    private readonly ICurrentUserService _currentUser;

    public MonthlyReportController(
        IMonthlyReportService reportService,
        IExcelExportService excelExport,
        IPdfExportService pdfExport,
        ICurrentUserService currentUser)
    {
        _reportService = reportService;
        _excelExport = excelExport;
        _pdfExport = pdfExport;
        _currentUser = currentUser;
    }

    [RequirePermission("Report.Monthly.View")]
    public IActionResult Index(int year = 0, int month = 0)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;
        ViewBag.Year = year;
        ViewBag.Month = month;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Report.Monthly.View")]
    public async Task<IActionResult> Generate(int year, int month)
    {
        var report = await _reportService.GenerateAsync(year, month);
        ViewBag.Year = year;
        ViewBag.Month = month;
        return View("Index", report);
    }

    [RequirePermission("Report.Monthly.View")]
    public async Task<IActionResult> ExportExcel(int year, int month)
    {
        var bytes = await _excelExport.ExportMonthlyReportAsync(_currentUser.CompanyId, year, month);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MonthlyReport_{year}_{month:D2}.xlsx");
    }

    [RequirePermission("Report.Monthly.View")]
    public async Task<IActionResult> ExportPdf(int year, int month)
    {
        var bytes = await _pdfExport.ExportMonthlyReportPdfAsync(_currentUser.CompanyId, year, month);
        return File(bytes, "application/pdf", $"MonthlyReport_{year}_{month:D2}.pdf");
    }
}
