using ERP.Web.Core.DTOs;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Web.Core.Interfaces.Repositories;

namespace ERP.Web.Areas.Attendance.Controllers;

[Area("Attendance")]
public class PunchController : Controller
{
    private readonly IPunchRecordService _punchService;
    private readonly IPunchImportService _importService;
    private readonly IEmployeeService _employeeService;

    public PunchController(IPunchRecordService punchService, IPunchImportService importService, IEmployeeService employeeService)
    {
        _punchService = punchService;
        _importService = importService;
        _employeeService = employeeService;
    }

    [RequirePermission("Attendance.Punch.Import")]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-7);
        var t = to ?? DateTime.Today;
        ViewBag.From = f; ViewBag.To = t;
        return View(await _punchService.GetByDateRangeAsync(f, t));
    }

    [RequirePermission("Attendance.Punch.Import")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = await _employeeService.GetAllAsync();
        return View(new PunchRecordCreateVm { PunchTime = DateTime.Now });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.Punch.Import")]
    public async Task<IActionResult> Create(PunchRecordCreateVm model)
    {
        var result = await _punchService.CreateManualAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Punch saved." : result.Error;
        return result.Success ? RedirectToAction(nameof(Index)) : View(model);
    }

    [RequirePermission("Attendance.Punch.Import")]
    public IActionResult Import() => View();

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.Punch.Import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0) { TempData["Error"] = "Select a file."; return View(); }
        using var stream = file.OpenReadStream();
        var result = await _importService.ImportAsync(stream);
        TempData["Success"] = $"Imported {result.Imported} records.";
        if (result.Errors.Any()) TempData["Error"] = string.Join("; ", result.Errors.Take(5));
        return RedirectToAction(nameof(Index));
    }
}

[Area("Attendance")]
public class AttendanceController : Controller
{
    private readonly IAttendanceProcessService _attendanceService;
    private readonly IOrganogramService _organogramService;
    private readonly IExcelExportService _excelExport;
    private readonly IPdfExportService _pdfExport;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJobCardService _jobCardService;
    private readonly IPayrollService _payrollService;

    public AttendanceController(
        IAttendanceProcessService attendanceService,
        IOrganogramService organogramService,
        IExcelExportService excelExport,
        IPdfExportService pdfExport,
        ICurrentUserService currentUserService,
        IJobCardService jobCardService,
        IPayrollService payrollService)
    {
        _attendanceService = attendanceService;
        _organogramService = organogramService;
        _excelExport = excelExport;
        _pdfExport = pdfExport;
        _currentUserService = currentUserService;
        _jobCardService = jobCardService;
        _payrollService = payrollService;
    }

    [RequirePermission("Attendance.Process")]
    public IActionResult Process() => View();

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.Process")]
    public async Task<IActionResult> Process(DateTime fromDate, DateTime toDate)
    {
        var result = await _attendanceService.ProcessAsync(fromDate, toDate);
        TempData[result.Error == null ? "Success" : "Error"] = result.Error ?? $"Processed {result.Processed}, skipped {result.Skipped}.";
        return RedirectToAction(nameof(Daily));
    }

    [RequirePermission("Attendance.Report.View")]
    public async Task<IActionResult> Daily([FromQuery] AttendanceFilterDto filter)
    {
        filter.From ??= DateTime.Today.AddDays(-30);
        filter.To ??= DateTime.Today;
        ViewBag.Filter = filter;
        ViewBag.FilterAction = Url.Action(nameof(Daily));
        ViewBag.FilterModule = "AttendanceDaily";
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync(filter.DepartmentId);
        return View(await _attendanceService.GetDailyAttendanceAsync(filter));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.Process")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _attendanceService.ApproveAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Approved." : result.Error;
        return RedirectToAction(nameof(Daily));
    }

    [RequirePermission("Attendance.Report.View")]
    public async Task<IActionResult> Report([FromQuery] AttendanceFilterDto filter)
    {
        filter.From ??= DateTime.Today.AddDays(-30);
        filter.To ??= DateTime.Today;
        ViewBag.Filter = filter;
        ViewBag.FilterAction = Url.Action(nameof(Report));
        ViewBag.FilterModule = "AttendanceReport";
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync(filter.DepartmentId);
        return View(await _attendanceService.GetReportAsync(filter));
    }

    [RequirePermission("Attendance.Report.View")]
    public async Task<IActionResult> Summary([FromQuery] AttendanceFilterDto filter)
    {
        filter.From ??= DateTime.Today.AddDays(-30);
        filter.To ??= DateTime.Today;
        ViewBag.Filter = filter;
        ViewBag.FilterAction = Url.Action(nameof(Summary));
        ViewBag.FilterModule = "AttendanceSummary";
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync(filter.DepartmentId);
        return View(await _attendanceService.GetSummaryAsync(filter));
    }

    [RequirePermission("Attendance.Report.View")]
    public async Task<IActionResult> ExportReport([FromQuery] AttendanceFilterDto filter)
    {
        var from = filter.From ?? DateTime.Today.AddDays(-30);
        var to = filter.To ?? DateTime.Today;
        var bytes = await _excelExport.ExportDailyAttendanceAsync(_currentUserService.CompanyId, from, to);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceReport.xlsx");
    }

    [RequirePermission("Attendance.Manpower.View")]
    public async Task<IActionResult> Manpower(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        ViewBag.Date = d;
        return View(await _attendanceService.GetManpowerReportAsync(d));
    }

    [RequirePermission("Attendance.JobCard.Manage")]
    public async Task<IActionResult> JobCard(DateTime? from, DateTime? to, int? lineId)
    {
        var f = from ?? DateTime.Today;
        var t = to ?? DateTime.Today;
        ViewBag.From = f;
        ViewBag.To = t;
        ViewBag.LineId = lineId;
        ViewBag.Lines = await _organogramService.GetLinesAsync();
        ViewBag.Employees = new List<object>();
        return View(await _jobCardService.GetByDateRangeAsync(f, t, lineId));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.JobCard.Manage")]
    public async Task<IActionResult> SaveJobCard(JobCardCreateVm model)
    {
        var result = await _jobCardService.SaveAsync(model);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Job card saved." : result.Error;
        return RedirectToAction(nameof(JobCard));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Attendance.JobCard.Manage")]
    public async Task<IActionResult> DeleteJobCard(int id)
    {
        var result = await _jobCardService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Deleted." : result.Error;
        return RedirectToAction(nameof(JobCard));
    }

    [RequirePermission("Attendance.AbsentStatus.View")]
    public async Task<IActionResult> AbsentStatus(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        ViewBag.Date = d;
        return View(await _attendanceService.GetAbsentStatusAsync(d));
    }

    [RequirePermission("Attendance.OT.View")]
    public async Task<IActionResult> OtDeduction(int? periodId)
    {
        var periods = await _organogramService.GetDepartmentsAsync();
        ViewBag.Periods = await _getPeriods();
        ViewBag.PeriodId = periodId;
        if (!periodId.HasValue) return View(Array.Empty<OtDeductionVm>());
        return View(await _attendanceService.GetOtDeductionAsync(periodId.Value));
    }

    [RequirePermission("Attendance.OT.View")]
    public async Task<IActionResult> DailyOvertime(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        ViewBag.Date = d;
        return View(await _attendanceService.GetDailyOvertimeAsync(d));
    }

    [RequirePermission("Attendance.OT.View")]
    public async Task<IActionResult> DailyOvertimeSummary(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-30);
        var t = to ?? DateTime.Today;
        ViewBag.From = f;
        ViewBag.To = t;
        return View(await _attendanceService.GetDailyOvertimeSummaryAsync(f, t));
    }

    [RequirePermission("Attendance.OT.View")]
    public async Task<IActionResult> ExportDailyOvertime(DateTime? from, DateTime? to)
    {
        var f = from ?? DateTime.Today.AddDays(-30);
        var t = to ?? DateTime.Today;
        var bytes = await _excelExport.ExportDailyOvertimeAsync(_currentUserService.CompanyId, f, t);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DailyOvertime_{f:yyyyMMdd}_{t:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Attendance.Manpower.View")]
    public async Task<IActionResult> ExportManpower(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        var bytes = await _excelExport.ExportManpowerAsync(_currentUserService.CompanyId, d);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Manpower_{d:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Attendance.JobCard.Manage")]
    public async Task<IActionResult> ExportJobCardPdf(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        var bytes = await _pdfExport.ExportJobCardPdfAsync(_currentUserService.CompanyId, d);
        return File(bytes, "application/pdf", $"JobCard_{d:yyyyMMdd}.pdf");
    }

    [RequirePermission("Attendance.AbsentStatus.View")]
    public async Task<IActionResult> ExportAbsentStatus(DateTime? date)
    {
        var d = date ?? DateTime.Today;
        var bytes = await _excelExport.ExportAbsentStatusAsync(_currentUserService.CompanyId, d);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AbsentStatus_{d:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Attendance.OT.View")]
    public async Task<IActionResult> ExportOtDeduction(int? periodId)
    {
        var pid = periodId ?? 0;
        if (pid == 0) return BadRequest();
        var bytes = await _excelExport.ExportOtDeductionAsync(_currentUserService.CompanyId, pid);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"OtDeduction_{pid}.xlsx");
    }

    [RequirePermission("Attendance.Report.View")]
    public async Task<IActionResult> ExportAttendanceReportPdf([FromQuery] AttendanceFilterDto filter)
    {
        var from = filter.From ?? DateTime.Today.AddDays(-30);
        var to = filter.To ?? DateTime.Today;
        var bytes = await _pdfExport.ExportAttendanceReportAsync(_currentUserService.CompanyId, from, to);
        return File(bytes, "application/pdf", $"AttendanceReport_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
    }

    private Task<IReadOnlyList<PayrollPeriodVm>> _getPeriods()
        => _payrollService.GetPeriodsAsync();
}
