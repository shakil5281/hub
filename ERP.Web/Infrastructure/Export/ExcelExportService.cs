using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ERP.Web.Infrastructure.Export;

public class ExcelExportService : IExcelExportService
{
    private readonly AppDbContext _context;

    public ExcelExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportEmployeesAsync(int companyId)
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Line)
            .Include(e => e.Designation)
            .Where(e => e.CompanyId == companyId)
            .OrderBy(e => e.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Employees");

        var headers = new[]
        {
            "Employee Code", "Punch Number", "Full Name", "Department", "Section", "Line",
            "Designation", "Joining Date", "Gross Salary", "Status"
        };

        for (var col = 1; col <= headers.Length; col++)
            sheet.Cells[1, col].Value = headers[col - 1];

        var row = 2;
        foreach (var e in employees)
        {
            sheet.Cells[row, 1].Value = e.EmployeeCode;
            sheet.Cells[row, 2].Value = e.PunchNumber;
            sheet.Cells[row, 3].Value = e.FullName;
            sheet.Cells[row, 4].Value = e.Department?.Name;
            sheet.Cells[row, 5].Value = e.Section?.Name;
            sheet.Cells[row, 6].Value = e.Line?.Name;
            sheet.Cells[row, 7].Value = e.Designation?.Title;
            sheet.Cells[row, 8].Value = e.JoiningDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 9].Value = e.GrossSalary;
            sheet.Cells[row, 10].Value = e.Status.ToString();
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportDailyAttendanceAsync(int companyId, DateTime fromDate, DateTime toDate)
    {
        var records = await _context.DailyAttendances
            .Include(a => a.Employee)
            .ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId
                && a.AttendanceDate >= fromDate.Date
                && a.AttendanceDate <= toDate.Date)
            .OrderBy(a => a.AttendanceDate)
            .ThenBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Daily Attendance");

        var headers = new[]
        {
            "Date", "Employee Code", "Employee Name", "Department", "In Time", "Out Time",
            "Late (min)", "OT (min)", "Absent", "Approved"
        };

        for (var col = 1; col <= headers.Length; col++)
            sheet.Cells[1, col].Value = headers[col - 1];

        var row = 2;
        foreach (var a in records)
        {
            sheet.Cells[row, 1].Value = a.AttendanceDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 2].Value = a.Employee?.EmployeeCode;
            sheet.Cells[row, 3].Value = a.Employee?.FullName;
            sheet.Cells[row, 4].Value = a.Employee?.Department?.Name;
            sheet.Cells[row, 5].Value = a.InTime?.ToString("yyyy-MM-dd HH:mm");
            sheet.Cells[row, 6].Value = a.OutTime?.ToString("yyyy-MM-dd HH:mm");
            sheet.Cells[row, 7].Value = a.LateMinutes;
            sheet.Cells[row, 8].Value = a.OvertimeMinutes;
            sheet.Cells[row, 9].Value = a.IsAbsent ? "Yes" : "No";
            sheet.Cells[row, 10].Value = a.IsApproved ? "Yes" : "No";
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportMonthlyPayrollAsync(int companyId, int payrollSheetId)
    {
        var details = await _context.PayrollDetails
            .Include(d => d.Employee)
            .ThenInclude(e => e!.Department)
            .Where(d => d.CompanyId == companyId && d.PayrollSheetId == payrollSheetId)
            .OrderBy(d => d.Employee!.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Payroll");

        var headers = new[]
        {
            "Employee Code", "Employee Name", "Department", "Gross", "Basic", "House Rent",
            "Medical", "Food", "Conveyance", "Payable Days", "Absent Ded.", "OT Amount",
            "Night Bill", "Holiday Bill", "Advance Ded.", "Net Payable"
        };

        for (var col = 1; col <= headers.Length; col++)
            sheet.Cells[1, col].Value = headers[col - 1];

        var row = 2;
        foreach (var d in details)
        {
            sheet.Cells[row, 1].Value = d.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = d.Employee?.FullName;
            sheet.Cells[row, 3].Value = d.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = d.GrossSalary;
            sheet.Cells[row, 5].Value = d.BasicSalary;
            sheet.Cells[row, 6].Value = d.HouseRent;
            sheet.Cells[row, 7].Value = d.MedicalAllowance;
            sheet.Cells[row, 8].Value = d.FoodAllowance;
            sheet.Cells[row, 9].Value = d.ConveyanceAllowance;
            sheet.Cells[row, 10].Value = d.AttendancePayableDays;
            sheet.Cells[row, 11].Value = d.AbsentDeduction;
            sheet.Cells[row, 12].Value = d.OvertimeAmount;
            sheet.Cells[row, 13].Value = d.NightBillAmount;
            sheet.Cells[row, 14].Value = d.HolidayBillAmount;
            sheet.Cells[row, 15].Value = d.AdvanceDeduction;
            sheet.Cells[row, 16].Value = d.NetPayableSalary;
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportDailySalarySheetAsync(int companyId, DateTime date)
    {
        var employees = await _context.Employees.Include(e => e.Department)
            .Where(e => e.CompanyId == companyId).OrderBy(e => e.EmployeeCode).ToListAsync();

        var attendances = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == date.Date).ToListAsync();
        var attMap = attendances.ToDictionary(a => a.EmployeeId);

        var from = new DateTime(date.Year, date.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var workingDays = CountWorkingDays(from, to);

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Daily Salary");
        var headers = new[] { "Code", "Name", "Department", "Gross Salary", "Working Days", "Daily Rate", "Status", "Payable Amount" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var e in employees)
        {
            attMap.TryGetValue(e.Id, out var att);
            var dailyRate = workingDays > 0 ? Math.Round(e.GrossSalary / workingDays, 2) : 0m;
            var isPresent = att != null && !att.IsAbsent && att.InTime.HasValue;
            sheet.Cells[row, 1].Value = e.EmployeeCode;
            sheet.Cells[row, 2].Value = e.FullName;
            sheet.Cells[row, 3].Value = e.Department?.Name;
            sheet.Cells[row, 4].Value = e.GrossSalary;
            sheet.Cells[row, 5].Value = workingDays;
            sheet.Cells[row, 6].Value = dailyRate;
            sheet.Cells[row, 7].Value = isPresent ? "Present" : (att?.IsHoliday == true ? "Holiday" : "Absent");
            sheet.Cells[row, 8].Value = isPresent ? dailyRate : 0m;
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportDailySalarySummaryAsync(int companyId, DateTime fromDate, DateTime toDate)
    {
        var employees = await _context.Employees.Where(e => e.CompanyId == companyId).ToListAsync();
        var attendances = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date >= fromDate.Date && a.AttendanceDate.Date <= toDate.Date).ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Daily Salary Summary");
        var headers = new[] { "Date", "Present", "Absent", "Holiday", "Total Payable", "Absent Deduction" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        for (var d = fromDate.Date; d <= toDate.Date; d = d.AddDays(1))
        {
            var workingDays = CountWorkingDays(new DateTime(d.Year, d.Month, 1), new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month)));
            var dayAtt = attendances.Where(a => a.AttendanceDate.Date == d).ToList();
            sheet.Cells[row, 1].Value = d.ToString("yyyy-MM-dd");
            sheet.Cells[row, 2].Value = dayAtt.Count(a => !a.IsAbsent && a.InTime.HasValue);
            sheet.Cells[row, 3].Value = dayAtt.Count(a => a.IsAbsent);
            sheet.Cells[row, 4].Value = dayAtt.Count(a => a.IsHoliday);
            sheet.Cells[row, 5].Value = employees.Sum(e => workingDays > 0 ? (dayAtt.Any(a => a.EmployeeId == e.Id && !a.IsAbsent) ? Math.Round(e.GrossSalary / workingDays, 2) : 0m) : 0m);
            sheet.Cells[row, 6].Value = employees.Sum(e => workingDays > 0 ? (dayAtt.Any(a => a.EmployeeId == e.Id && a.IsAbsent) ? Math.Round(e.GrossSalary / workingDays, 2) : 0m) : 0m);
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportEidBonusAsync(int companyId, int year, int bonusType)
    {
        var query = _context.EidBonuses.Include(e => e.Employee).ThenInclude(emp => emp!.Department)
            .Where(e => e.CompanyId == companyId && e.Year == year);
        if (bonusType > 0) query = query.Where(e => (int)e.BonusType == bonusType);
        var bonuses = await query.OrderBy(e => e.Employee!.EmployeeCode).ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Eid Bonus");
        var headers = new[] { "Code", "Name", "Department", "Bonus Type", "Year", "Amount", "Status" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var b in bonuses)
        {
            sheet.Cells[row, 1].Value = b.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = b.Employee?.FullName;
            sheet.Cells[row, 3].Value = b.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = b.BonusType.ToString();
            sheet.Cells[row, 5].Value = b.Year;
            sheet.Cells[row, 6].Value = b.BonusAmount;
            sheet.Cells[row, 7].Value = b.BonusStatus.ToString();
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportManpowerAsync(int companyId, DateTime date)
    {
        var attendances = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.Employee).ThenInclude(e => e!.Section)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == date.Date)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Manpower");
        var headers = new[] { "Department", "Section", "Total", "Present", "Absent" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        var grouped = attendances.GroupBy(a => new { Dept = a.Employee?.Department?.Name ?? "-", Sec = a.Employee?.Section?.Name ?? "-" });
        foreach (var g in grouped.OrderBy(g => g.Key.Dept))
        {
            sheet.Cells[row, 1].Value = g.Key.Dept;
            sheet.Cells[row, 2].Value = g.Key.Sec;
            sheet.Cells[row, 3].Value = g.Count();
            sheet.Cells[row, 4].Value = g.Count(a => !a.IsAbsent && a.InTime.HasValue);
            sheet.Cells[row, 5].Value = g.Count(a => a.IsAbsent);
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportDailyOvertimeAsync(int companyId, DateTime fromDate, DateTime toDate)
    {
        var data = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date >= fromDate.Date && a.AttendanceDate.Date <= toDate.Date && a.OvertimeMinutes > 0)
            .OrderBy(a => a.AttendanceDate).ThenBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Daily Overtime");
        var headers = new[] { "Date", "Code", "Name", "Department", "OT Minutes", "OT Hours" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var a in data)
        {
            sheet.Cells[row, 1].Value = a.AttendanceDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 2].Value = a.Employee?.EmployeeCode;
            sheet.Cells[row, 3].Value = a.Employee?.FullName;
            sheet.Cells[row, 4].Value = a.Employee?.Department?.Name;
            sheet.Cells[row, 5].Value = a.OvertimeMinutes;
            sheet.Cells[row, 6].Value = Math.Round(a.OvertimeMinutes / 60m, 2);
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportMonthlyReportAsync(int companyId, int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        var attendances = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate >= from && a.AttendanceDate <= to)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Monthly Report");
        var headers = new[] { "Department", "Employees", "Present Days", "Absent Days", "OT Hours" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        var grouped = attendances.GroupBy(a => a.Employee?.Department?.Name ?? "Unassigned");
        foreach (var g in grouped.OrderBy(g => g.Key))
        {
            var empIds = g.Select(a => a.EmployeeId).Distinct().Count();
            sheet.Cells[row, 1].Value = g.Key;
            sheet.Cells[row, 2].Value = empIds;
            sheet.Cells[row, 3].Value = g.Count(a => !a.IsAbsent && a.InTime.HasValue);
            sheet.Cells[row, 4].Value = g.Count(a => a.IsAbsent);
            sheet.Cells[row, 5].Value = Math.Round(g.Sum(a => a.OvertimeMinutes) / 60m, 2);
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportPayrollSummaryAsync(int companyId, int sheetId)
    {
        var details = await _context.PayrollDetails
            .Include(d => d.Employee).ThenInclude(e => e!.Department)
            .Where(d => d.PayrollSheetId == sheetId && d.CompanyId == companyId)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Payroll Summary");
        var headers = new[] { "Department", "Employees", "Total Gross", "Total Net", "Total OT", "Night Bill", "Holiday Bill" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];

        var grouped = details.GroupBy(d => d.Employee?.Department?.Name ?? "Unassigned").OrderBy(g => g.Key);
        var row = 2;
        foreach (var g in grouped)
        {
            sheet.Cells[row, 1].Value = g.Key;
            sheet.Cells[row, 2].Value = g.Count();
            sheet.Cells[row, 3].Value = g.Sum(x => x.GrossSalary);
            sheet.Cells[row, 4].Value = g.Sum(x => x.NetPayableSalary);
            sheet.Cells[row, 5].Value = g.Sum(x => x.OvertimeAmount);
            sheet.Cells[row, 6].Value = g.Sum(x => x.NightBillAmount);
            sheet.Cells[row, 7].Value = g.Sum(x => x.HolidayBillAmount);
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportAbsentStatusAsync(int companyId, DateTime date)
    {
        var absents = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.Employee).ThenInclude(e => e!.Section)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == date.Date && a.IsAbsent)
            .OrderBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        var absentEmpIds = absents.Select(a => a.EmployeeId).ToHashSet();
        var leaves = await _context.LeaveApplications
            .Include(l => l.LeaveType)
            .Where(l => l.CompanyId == companyId && l.ApprovalStatus == Core.Enums.LeaveApprovalStatus.Approved
                && l.FromDate.Date <= date.Date && l.ToDate.Date >= date.Date
                && absentEmpIds.Contains(l.EmployeeId))
            .ToListAsync();
        var leaveMap = leaves.ToDictionary(l => l.EmployeeId, l => l.LeaveType?.Name ?? "Leave");
        var isHoliday = await _context.Holidays.AnyAsync(h => h.CompanyId == companyId && h.HolidayDate.Date == date.Date);

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Absent Status");
        var headers = new[] { "Code", "Name", "Department", "Section", "Date", "On Leave", "Leave Type", "Holiday" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var a in absents)
        {
            sheet.Cells[row, 1].Value = a.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = a.Employee?.FullName;
            sheet.Cells[row, 3].Value = a.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = a.Employee?.Section?.Name;
            sheet.Cells[row, 5].Value = a.AttendanceDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 6].Value = leaveMap.ContainsKey(a.EmployeeId) ? "Yes" : "No";
            sheet.Cells[row, 7].Value = leaveMap.TryGetValue(a.EmployeeId, out var lt) ? lt : "-";
            sheet.Cells[row, 8].Value = (a.IsHoliday || isHoliday) ? "Yes" : "No";
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportOtDeductionAsync(int companyId, int periodId)
    {
        var details = await _context.PayrollDetails
            .Include(d => d.Employee).ThenInclude(e => e!.Department)
            .Where(d => d.PayrollSheet!.PayrollPeriodId == periodId && d.CompanyId == companyId)
            .OrderBy(d => d.Employee!.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("OT Deduction");
        var headers = new[] { "Code", "Name", "Department", "OT Amount", "Absent Deduction", "Net Adjustment" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var d in details)
        {
            sheet.Cells[row, 1].Value = d.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = d.Employee?.FullName;
            sheet.Cells[row, 3].Value = d.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = d.OvertimeAmount;
            sheet.Cells[row, 5].Value = d.AbsentDeduction;
            sheet.Cells[row, 6].Value = d.OvertimeAmount - d.AbsentDeduction;
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportHolidaysAsync(int companyId, int year)
    {
        var holidays = await _context.Holidays
            .Where(h => h.CompanyId == companyId && h.HolidayDate.Year == year)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Holidays");
        var headers = new[] { "Name", "Date", "Day", "Type", "Description" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var h in holidays)
        {
            sheet.Cells[row, 1].Value = h.Name;
            sheet.Cells[row, 2].Value = h.HolidayDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 3].Value = h.HolidayDate.DayOfWeek.ToString();
            sheet.Cells[row, 4].Value = h.HolidayType.ToString();
            sheet.Cells[row, 5].Value = h.Description;
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportMonthlyLeaveRecordAsync(int companyId, int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        var applications = await _context.LeaveApplications
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.LeaveType)
            .Where(a => a.CompanyId == companyId && a.FromDate <= to && a.ToDate >= from)
            .OrderBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Monthly Leave");
        var headers = new[] { "Code", "Name", "Department", "Leave Type", "From", "To", "Days", "Status" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var a in applications)
        {
            sheet.Cells[row, 1].Value = a.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = a.Employee?.FullName;
            sheet.Cells[row, 3].Value = a.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = a.LeaveType?.Name;
            sheet.Cells[row, 5].Value = a.FromDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 6].Value = a.ToDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 7].Value = a.TotalDays;
            sheet.Cells[row, 8].Value = a.ApprovalStatus.ToString();
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportEarnLeaveAsync(int companyId, int year)
    {
        var earnLeaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(t => t.CompanyId == companyId && t.Name.ToLower().Contains("earn"));

        var query = _context.LeaveBalances
            .Include(b => b.Employee).ThenInclude(e => e!.Department)
            .Where(b => b.CompanyId == companyId && b.Year == year);
        if (earnLeaveType != null)
            query = query.Where(b => b.LeaveTypeId == earnLeaveType.Id);

        var balances = await query.OrderBy(b => b.Employee!.EmployeeCode).ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Earn Leave");
        var headers = new[] { "Code", "Name", "Department", "Year", "Allocated", "Used", "Remaining" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];
        var row = 2;
        foreach (var b in balances)
        {
            sheet.Cells[row, 1].Value = b.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = b.Employee?.FullName;
            sheet.Cells[row, 3].Value = b.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = b.Year;
            sheet.Cells[row, 5].Value = b.AllocatedDays;
            sheet.Cells[row, 6].Value = b.UsedDays;
            sheet.Cells[row, 7].Value = b.RemainingDays;
            row++;
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportVacancyReportAsync(int companyId, int year, int month)
    {
        var plans = await _context.StaffingPlans
            .Include(s => s.Department).Include(s => s.Section).Include(s => s.Line).Include(s => s.Designation)
            .Where(s => s.CompanyId == companyId && s.Year == year && s.Month == month)
            .OrderBy(s => s.Department!.Name)
            .ToListAsync();

        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId && e.Status == EntityStatus.Active)
            .ToListAsync();

        var approvedHiring = await _context.HiringRequests
            .Where(h => h.CompanyId == companyId &&
                h.HiringRequestStatus == HiringRequestStatus.Approved &&
                h.RequestDate.Year == year &&
                h.RequestDate.Month == month)
            .ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Vacancy Report");
        var headers = new[] { "Department", "Section", "Line", "Designation", "Required", "Actual", "Incoming", "Vacancy" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];

        var row = 2;
        foreach (var plan in plans)
        {
            var actual = employees.Count(e =>
                e.DepartmentId == plan.DepartmentId &&
                (plan.SectionId == null || e.SectionId == plan.SectionId) &&
                (plan.LineId == null || e.LineId == plan.LineId) &&
                (plan.DesignationId == null || e.DesignationId == plan.DesignationId));

            var incoming = approvedHiring
                .Where(h =>
                    h.DepartmentId == plan.DepartmentId &&
                    (plan.SectionId == null || h.SectionId == plan.SectionId) &&
                    (plan.LineId == null || h.LineId == plan.LineId) &&
                    (plan.DesignationId == null || h.DesignationId == plan.DesignationId))
                .Sum(h => h.RequestedCount);

            sheet.Cells[row, 1].Value = plan.Department?.Name;
            sheet.Cells[row, 2].Value = plan.Section?.Name;
            sheet.Cells[row, 3].Value = plan.Line?.Name;
            sheet.Cells[row, 4].Value = plan.Designation?.Title;
            sheet.Cells[row, 5].Value = plan.RequiredCount;
            sheet.Cells[row, 6].Value = actual;
            sheet.Cells[row, 7].Value = incoming;
            sheet.Cells[row, 8].Value = Math.Max(0, plan.RequiredCount - actual);
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportHiringRequestsAsync(int companyId, HiringRequestStatus? status)
    {
        var query = _context.HiringRequests
            .Include(h => h.Department).Include(h => h.Section).Include(h => h.Line).Include(h => h.Designation)
            .Where(h => h.CompanyId == companyId);
        if (status.HasValue) query = query.Where(h => h.HiringRequestStatus == status.Value);
        var requests = await query.OrderByDescending(h => h.RequestDate).ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Hiring Requests");
        var headers = new[] { "Department", "Section", "Line", "Designation", "Count", "Request Date", "Target Join", "Status", "Reason" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];

        var row = 2;
        foreach (var h in requests)
        {
            sheet.Cells[row, 1].Value = h.Department?.Name;
            sheet.Cells[row, 2].Value = h.Section?.Name;
            sheet.Cells[row, 3].Value = h.Line?.Name;
            sheet.Cells[row, 4].Value = h.Designation?.Title;
            sheet.Cells[row, 5].Value = h.RequestedCount;
            sheet.Cells[row, 6].Value = h.RequestDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 7].Value = h.TargetJoinDate?.ToString("yyyy-MM-dd");
            sheet.Cells[row, 8].Value = h.HiringRequestStatus.ToString();
            sheet.Cells[row, 9].Value = h.Reason;
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportSeparationsAsync(int companyId, DateTime? from, DateTime? to, SeparationType? separationType, int? departmentId)
    {
        var query = _context.EmployeeSeparations
            .Include(s => s.Employee).ThenInclude(e => e!.Department)
            .Where(s => s.CompanyId == companyId);
        if (from.HasValue) query = query.Where(s => s.SeparationDate >= from.Value.Date);
        if (to.HasValue) query = query.Where(s => s.SeparationDate <= to.Value.Date);
        if (separationType.HasValue) query = query.Where(s => s.SeparationType == separationType.Value);
        if (departmentId.HasValue) query = query.Where(s => s.Employee!.DepartmentId == departmentId.Value);
        var separations = await query.OrderByDescending(s => s.SeparationDate).ToListAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Separations");
        var headers = new[] { "Code", "Name", "Department", "Type", "Separation Date", "Notice Date", "Reason", "Remarks", "Recorded" };
        for (var i = 0; i < headers.Length; i++) sheet.Cells[1, i + 1].Value = headers[i];

        var row = 2;
        foreach (var s in separations)
        {
            sheet.Cells[row, 1].Value = s.Employee?.EmployeeCode;
            sheet.Cells[row, 2].Value = s.Employee?.FullName;
            sheet.Cells[row, 3].Value = s.Employee?.Department?.Name;
            sheet.Cells[row, 4].Value = s.SeparationType.ToString();
            sheet.Cells[row, 5].Value = s.SeparationDate.ToString("yyyy-MM-dd");
            sheet.Cells[row, 6].Value = s.NoticeDate?.ToString("yyyy-MM-dd");
            sheet.Cells[row, 7].Value = s.Reason;
            sheet.Cells[row, 8].Value = s.Remarks;
            sheet.Cells[row, 9].Value = s.CreatedAt.ToString("yyyy-MM-dd");
            row++;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    private static int CountWorkingDays(DateTime start, DateTime end)
    {
        var count = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            if (d.DayOfWeek != DayOfWeek.Friday) count++;
        return Math.Max(count, 1);
    }
}
