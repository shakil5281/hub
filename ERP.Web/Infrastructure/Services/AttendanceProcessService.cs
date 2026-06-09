using ERP.Web.Core.DTOs;
using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ERP.Web.Core.Enums;

namespace ERP.Web.Infrastructure.Services;

public class AttendanceProcessService : IAttendanceProcessService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AttendanceProcessService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<(int Processed, int Skipped, string? Error)> ProcessAsync(DateTime from, DateTime to)
    {
        var companyId = _currentUserService.CompanyId;
        var fromDate = from.Date;
        var toDate = to.Date;

        if (fromDate > toDate)
            return (0, 0, "From date cannot be after to date.");

        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        var assignments = await _context.ShiftAssignments
            .Include(a => a.Shift)
            .Where(a => a.CompanyId == companyId)
            .ToListAsync();

        var windowStart = fromDate.AddDays(-1);
        var windowEnd = toDate.AddDays(1).AddTicks(-1);
        var punches = await _context.PunchRecords
            .Where(p => p.CompanyId == companyId && p.PunchTime >= windowStart && p.PunchTime <= windowEnd)
            .OrderBy(p => p.PunchTime)
            .ToListAsync();

        var existingAttendance = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId && a.AttendanceDate >= fromDate && a.AttendanceDate <= toDate)
            .ToListAsync();

        var existingMap = existingAttendance.ToDictionary(a => (a.EmployeeId, a.AttendanceDate.Date));

        var processed = 0;
        var skipped = 0;

        for (var date = fromDate; date <= toDate; date = date.AddDays(1))
        {
            foreach (var employee in employees)
            {
                if (existingMap.TryGetValue((employee.Id, date), out var existing)
                    && (existing.IsApproved || existing.IsPayrollLocked))
                {
                    skipped++;
                    continue;
                }

                var shift = GetEffectiveShift(assignments, employee.Id, date);
                var (windowFrom, windowTo) = GetShiftWindow(date, shift);
                var dayPunches = punches
                    .Where(p => p.EmployeeId == employee.Id && p.PunchTime >= windowFrom && p.PunchTime <= windowTo)
                    .OrderBy(p => p.PunchTime)
                    .ToList();

                var attendance = existing ?? new DailyAttendance
                {
                    CompanyId = companyId,
                    EmployeeId = employee.Id,
                    AttendanceDate = date,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _currentUserService.UserName
                };

                if (existing == null)
                    _context.DailyAttendances.Add(attendance);

                ApplyAttendanceCalculation(attendance, shift, date, dayPunches);
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.UpdatedBy = _currentUserService.UserName;

                processed++;
            }
        }

        _context.AttendanceProcessLogs.Add(new AttendanceProcessLog
        {
            CompanyId = companyId,
            FromDate = fromDate,
            ToDate = toDate,
            ProcessedRows = processed,
            SkippedRows = skipped,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserName
        });

        await _context.SaveChangesAsync();
        return (processed, skipped, null);
    }

    public async Task<IReadOnlyList<DailyAttendanceVm>> GetDailyAttendanceAsync(AttendanceFilterDto filter)
    {
        var from = filter.From?.Date ?? DateTime.Today.AddDays(-30);
        var to = filter.To?.Date ?? DateTime.Today;
        var query = ApplyAttendanceFilter(_context.DailyAttendances
            .Include(a => a.Employee)
            .ThenInclude(e => e!.Department)
            .AsQueryable(), filter, from, to);

        return await query
            .OrderBy(a => a.AttendanceDate)
            .ThenBy(a => a.Employee!.FullName)
            .Select(a => new DailyAttendanceVm(
                a.Id,
                a.Employee!.FullName,
                a.AttendanceDate,
                a.InTime,
                a.OutTime,
                a.LateMinutes,
                a.OvertimeMinutes,
                a.IsAbsent,
                a.IsApproved))
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> ApproveAsync(int id)
    {
        var attendance = await _context.DailyAttendances
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == _currentUserService.CompanyId);
        if (attendance == null) return (false, "Attendance record not found.");
        if (attendance.IsPayrollLocked) return (false, "Record is payroll locked.");

        attendance.IsApproved = true;
        attendance.UpdatedAt = DateTime.UtcNow;
        attendance.UpdatedBy = _currentUserService.UserName;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<AttendanceReportVm>> GetReportAsync(AttendanceFilterDto filter)
    {
        var from = filter.From?.Date ?? DateTime.Today.AddDays(-30);
        var to = filter.To?.Date ?? DateTime.Today;
        var query = ApplyAttendanceFilter(_context.DailyAttendances
            .Include(a => a.Employee)
            .ThenInclude(e => e!.Department)
            .AsQueryable(), filter, from, to);

        return await query
            .OrderBy(a => a.AttendanceDate)
            .ThenBy(a => a.Employee!.EmployeeCode)
            .Select(a => new AttendanceReportVm(
                a.Employee!.EmployeeCode,
                a.Employee.FullName,
                a.Employee.Department!.Name,
                a.AttendanceDate,
                a.InTime,
                a.OutTime,
                a.LateMinutes,
                a.OvertimeMinutes,
                a.IsAbsent))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AttendanceSummaryVm>> GetSummaryAsync(AttendanceFilterDto filter)
    {
        var from = filter.From?.Date ?? DateTime.Today.AddDays(-30);
        var to = filter.To?.Date ?? DateTime.Today;
        var query = ApplyAttendanceFilter(_context.DailyAttendances
            .Include(a => a.Employee)
            .ThenInclude(e => e!.Department)
            .AsQueryable(), filter, from, to);

        return await query
            .GroupBy(a => new { a.EmployeeId, a.Employee!.EmployeeCode, a.Employee.FullName, Department = a.Employee.Department!.Name })
            .Select(g => new AttendanceSummaryVm(
                g.Key.EmployeeCode,
                g.Key.FullName,
                g.Key.Department,
                g.Count(x => !x.IsAbsent && x.InTime.HasValue),
                g.Count(x => x.IsAbsent),
                g.Count(x => x.LateMinutes > 0),
                Math.Round(g.Sum(x => x.OvertimeMinutes) / 60m, 2)))
            .OrderBy(s => s.EmployeeCode)
            .ToListAsync();
    }

    private IQueryable<DailyAttendance> ApplyAttendanceFilter(IQueryable<DailyAttendance> query, AttendanceFilterDto filter, DateTime from, DateTime to)
    {
        query = query.Where(a => a.CompanyId == _currentUserService.CompanyId
            && a.AttendanceDate >= from && a.AttendanceDate <= to);

        if (filter.DepartmentId.HasValue)
            query = query.Where(a => a.Employee!.DepartmentId == filter.DepartmentId);
        if (filter.SectionId.HasValue)
            query = query.Where(a => a.Employee!.SectionId == filter.SectionId);
        if (filter.LineId.HasValue)
            query = query.Where(a => a.Employee!.LineId == filter.LineId);
        if (filter.IsAbsent.HasValue)
            query = query.Where(a => a.IsAbsent == filter.IsAbsent);
        if (filter.IsApproved.HasValue)
            query = query.Where(a => a.IsApproved == filter.IsApproved);
        if (filter.HasLate == true)
            query = query.Where(a => a.LateMinutes > 0);
        if (filter.HasOvertime == true)
            query = query.Where(a => a.OvertimeMinutes > 0);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(a =>
                a.Employee!.FullName.ToLower().Contains(term) ||
                a.Employee.EmployeeCode.ToLower().Contains(term));
        }
        return query;
    }

    public async Task<IReadOnlyList<ManpowerVm>> GetManpowerReportAsync(DateTime date)
    {
        var companyId = _currentUserService.CompanyId;
        var dateOnly = date.Date;

        var attendances = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.Employee).ThenInclude(e => e!.Section)
            .Include(a => a.Employee).ThenInclude(e => e!.Line)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == dateOnly)
            .ToListAsync();

        var employees = await _context.Employees
            .Include(e => e.Department).Include(e => e.Section).Include(e => e.Line)
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        var leaveApplications = await _context.LeaveApplications
            .Where(l => l.CompanyId == companyId && l.ApprovalStatus == LeaveApprovalStatus.Approved
                && l.FromDate.Date <= dateOnly && l.ToDate.Date >= dateOnly)
            .ToListAsync();

        var holidays = await _context.Holidays
            .Where(h => h.CompanyId == companyId && h.HolidayDate.Date == dateOnly)
            .AnyAsync();

        var result = employees
            .GroupBy(e => new { Dept = e.Department?.Name ?? "Unassigned", Sec = e.Section?.Name ?? "-", Ln = e.Line?.Name ?? "-" })
            .Select(g =>
            {
                var empIds = g.Select(e => e.Id).ToHashSet();
                var att = attendances.Where(a => empIds.Contains(a.EmployeeId)).ToList();
                var onLeave = leaveApplications.Count(l => empIds.Contains(l.EmployeeId));
                return new ManpowerVm(
                    g.Key.Dept, g.Key.Sec, g.Key.Ln,
                    g.Count(),
                    att.Count(a => !a.IsAbsent && a.InTime.HasValue),
                    att.Count(a => a.IsAbsent),
                    onLeave,
                    holidays ? g.Count() : 0);
            })
            .OrderBy(m => m.Department).ThenBy(m => m.Section)
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<AbsentStatusVm>> GetAbsentStatusAsync(DateTime date)
    {
        var companyId = _currentUserService.CompanyId;
        var dateOnly = date.Date;

        var absents = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.Employee).ThenInclude(e => e!.Section)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == dateOnly && a.IsAbsent)
            .ToListAsync();

        var absentEmpIds = absents.Select(a => a.EmployeeId).ToHashSet();

        var leaveApplications = await _context.LeaveApplications
            .Include(l => l.LeaveType)
            .Where(l => l.CompanyId == companyId && l.ApprovalStatus == LeaveApprovalStatus.Approved
                && l.FromDate.Date <= dateOnly && l.ToDate.Date >= dateOnly
                && absentEmpIds.Contains(l.EmployeeId))
            .ToListAsync();

        var isHoliday = await _context.Holidays
            .AnyAsync(h => h.CompanyId == companyId && h.HolidayDate.Date == dateOnly);

        var leaveMap = leaveApplications.ToDictionary(l => l.EmployeeId, l => l.LeaveType?.Name ?? "Leave");

        return absents.Select(a => new AbsentStatusVm(
            a.Employee!.EmployeeCode,
            a.Employee.FullName,
            a.Employee.Department?.Name ?? "-",
            a.Employee.Section?.Name ?? "-",
            a.AttendanceDate,
            leaveMap.ContainsKey(a.EmployeeId),
            a.IsHoliday || isHoliday,
            leaveMap.TryGetValue(a.EmployeeId, out var lt) ? lt : "-"))
            .OrderBy(v => v.Department).ThenBy(v => v.EmployeeCode)
            .ToList();
    }

    public async Task<IReadOnlyList<DailyOvertimeVm>> GetDailyOvertimeAsync(DateTime date)
    {
        var companyId = _currentUserService.CompanyId;
        var dateOnly = date.Date;

        var billRates = await _context.BillRateConfigs.Where(b => b.CompanyId == companyId).ToListAsync();
        var otRate = billRates.FirstOrDefault(b => b.BillType == BillType.Night)?.Amount ?? 0m;

        return await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == dateOnly && a.OvertimeMinutes > 0)
            .OrderBy(a => a.Employee!.EmployeeCode)
            .Select(a => new DailyOvertimeVm(
                a.Employee!.EmployeeCode,
                a.Employee.FullName,
                a.Employee.Department!.Name,
                a.AttendanceDate,
                a.InTime,
                a.OutTime,
                a.OvertimeMinutes,
                Math.Round(a.OvertimeMinutes / 60m, 2),
                Math.Round(a.OvertimeMinutes / 60m * otRate, 2)))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<DailyOvertimeSummaryVm>> GetDailyOvertimeSummaryAsync(DateTime from, DateTime to)
    {
        var companyId = _currentUserService.CompanyId;
        var fromDate = from.Date;
        var toDate = to.Date;

        var billRates = await _context.BillRateConfigs.Where(b => b.CompanyId == companyId).ToListAsync();
        var otRate = billRates.FirstOrDefault(b => b.BillType == BillType.Night)?.Amount ?? 0m;

        var data = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date >= fromDate && a.AttendanceDate.Date <= toDate && a.OvertimeMinutes > 0)
            .ToListAsync();

        return data.GroupBy(a => new { a.EmployeeId, Code = a.Employee!.EmployeeCode, Name = a.Employee.FullName, Dept = a.Employee.Department!.Name })
            .Select(g =>
            {
                var totalMin = g.Sum(x => x.OvertimeMinutes);
                var totalHrs = Math.Round(totalMin / 60m, 2);
                return new DailyOvertimeSummaryVm(
                    g.Key.Code, g.Key.Name, g.Key.Dept,
                    totalMin, totalHrs,
                    Math.Round(totalHrs * otRate, 2),
                    g.Count());
            })
            .OrderBy(v => v.Department).ThenBy(v => v.EmployeeCode)
            .ToList();
    }

    public async Task<IReadOnlyList<OtDeductionVm>> GetOtDeductionAsync(int periodId)
    {
        var companyId = _currentUserService.CompanyId;
        var period = await _context.PayrollPeriods.FirstOrDefaultAsync(p => p.Id == periodId && p.CompanyId == companyId);
        if (period == null) return Array.Empty<OtDeductionVm>();

        var billRates = await _context.BillRateConfigs.Where(b => b.CompanyId == companyId).ToListAsync();
        var otRate = billRates.FirstOrDefault(b => b.BillType == BillType.Night)?.Amount ?? 0m;

        var details = await _context.PayrollDetails
            .Include(d => d.Employee).ThenInclude(e => e!.Department)
            .Where(d => d.PayrollSheet!.PayrollPeriodId == periodId && d.CompanyId == companyId)
            .ToListAsync();

        return details.Select(d => new OtDeductionVm(
            d.Employee!.EmployeeCode,
            d.Employee.FullName,
            d.Employee.Department?.Name ?? "-",
            (int)(d.OvertimeAmount / (otRate > 0 ? otRate : 1) * 60),
            d.OvertimeAmount,
            d.AbsentDeduction,
            d.OvertimeAmount - d.AbsentDeduction))
            .OrderBy(v => v.Department).ThenBy(v => v.EmployeeCode)
            .ToList();
    }

    private static Shift? GetEffectiveShift(IReadOnlyList<ShiftAssignment> assignments, int employeeId, DateTime date)
    {
        return assignments
            .Where(a => a.EmployeeId == employeeId
                && a.EffectiveFrom.Date <= date
                && (!a.EffectiveTo.HasValue || a.EffectiveTo.Value.Date >= date))
            .OrderByDescending(a => a.EffectiveFrom)
            .Select(a => a.Shift)
            .FirstOrDefault();
    }

    private static (DateTime WindowFrom, DateTime WindowTo) GetShiftWindow(DateTime attendanceDate, Shift? shift)
    {
        var startTime = shift?.StartTime ?? new TimeSpan(8, 0, 0);
        var beforeMinutes = shift?.PunchWindowBeforeMinutes ?? 60;

        var windowFrom = attendanceDate.Date.Add(startTime).AddMinutes(-beforeMinutes);
        var windowTo = attendanceDate.Date.AddDays(1).Add(startTime).AddMinutes(-1);
        return (windowFrom, windowTo);
    }

    private static void ApplyAttendanceCalculation(
        DailyAttendance attendance,
        Shift? shift,
        DateTime attendanceDate,
        IReadOnlyList<PunchRecord> dayPunches)
    {
        attendance.IsHoliday = attendanceDate.DayOfWeek == DayOfWeek.Friday;
        attendance.IsNightShift = shift != null && shift.EndTime < shift.StartTime;

        if (dayPunches.Count == 0)
        {
            attendance.InTime = null;
            attendance.OutTime = null;
            attendance.LateMinutes = 0;
            attendance.OvertimeMinutes = 0;
            attendance.WorkedMinutes = 0;
            attendance.IsAbsent = true;
            return;
        }

        attendance.InTime = dayPunches.First().PunchTime;
        attendance.OutTime = dayPunches.Last().PunchTime;
        attendance.IsAbsent = attendance.InTime == null || attendance.OutTime == null;

        if (attendance.IsAbsent)
        {
            attendance.LateMinutes = 0;
            attendance.OvertimeMinutes = 0;
            attendance.WorkedMinutes = 0;
            return;
        }

        var startTime = shift?.StartTime ?? new TimeSpan(8, 0, 0);
        var endTime = shift?.EndTime ?? new TimeSpan(17, 0, 0);
        var graceMinutes = shift?.GraceMinutes ?? 0;
        var maxOt = shift?.MaxOvertimeMinutes ?? 120;

        var expectedIn = attendanceDate.Date.Add(startTime).AddMinutes(graceMinutes);
        attendance.LateMinutes = attendance.InTime > expectedIn
            ? (int)(attendance.InTime.Value - expectedIn).TotalMinutes
            : 0;

        var expectedOut = endTime < startTime
            ? attendanceDate.Date.AddDays(1).Add(endTime)
            : attendanceDate.Date.Add(endTime);

        var overtime = attendance.OutTime > expectedOut
            ? (int)(attendance.OutTime.Value - expectedOut).TotalMinutes
            : 0;
        attendance.OvertimeMinutes = Math.Min(Math.Max(0, overtime), maxOt);

        attendance.WorkedMinutes = (int)(attendance.OutTime!.Value - attendance.InTime!.Value).TotalMinutes;
    }
}
