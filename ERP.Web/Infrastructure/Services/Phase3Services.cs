using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class TemporaryShiftService : ITemporaryShiftService
{
    private readonly ITemporaryShiftRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public TemporaryShiftService(ITemporaryShiftRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<TemporaryShiftVm>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var list = await _repository.GetByDateRangeAsync(_currentUserService.CompanyId, from, to);
        return list.Select(t => new TemporaryShiftVm(
            t.Id,
            t.EmployeeId,
            t.Employee?.FullName ?? string.Empty,
            t.Employee?.EmployeeCode ?? string.Empty,
            t.Employee?.Department?.Name ?? "-",
            t.ShiftId,
            t.Shift?.Name ?? string.Empty,
            t.AssignmentDate,
            t.Reason)).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveAsync(TemporaryShiftCreateVm model)
    {
        var companyId = _currentUserService.CompanyId;
        if (model.Id == 0)
        {
            await _repository.AddAsync(new TemporaryShiftAssignment
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                ShiftId = model.ShiftId,
                AssignmentDate = model.AssignmentDate.Date,
                Reason = model.Reason?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var existing = await _repository.GetByIdAsync(model.Id, companyId);
            if (existing == null) return (false, "Assignment not found.");
            existing.EmployeeId = model.EmployeeId;
            existing.ShiftId = model.ShiftId;
            existing.AssignmentDate = model.AssignmentDate.Date;
            existing.Reason = model.Reason?.Trim();
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUserService.UserName;
        }
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var assignment = await _repository.GetByIdAsync(id, _currentUserService.CompanyId);
        if (assignment == null) return (false, "Assignment not found.");
        assignment.IsDeleted = true;
        assignment.UpdatedAt = DateTime.UtcNow;
        assignment.UpdatedBy = _currentUserService.UserName;
        await _repository.SaveChangesAsync();
        return (true, null);
    }
}

public class EidBonusService : IEidBonusService
{
    private readonly IEidBonusRepository _repository;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public EidBonusService(IEidBonusRepository repository, AppDbContext context, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<EidBonusVm>> GetByYearAsync(int year, int bonusType)
    {
        var list = await _repository.GetByYearAsync(_currentUserService.CompanyId, year);
        if (bonusType > 0)
            list = list.Where(b => (int)b.BonusType == bonusType).ToList();
        return list.Select(MapVm).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveAsync(EidBonusVm model)
    {
        var companyId = _currentUserService.CompanyId;
        if (model.Id == 0)
        {
            await _repository.AddAsync(new EidBonus
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                BonusType = (BonusType)model.BonusType,
                Year = model.Year,
                BonusAmount = model.BonusAmount,
                BonusStatus = BonusStatus.Draft,
                Remarks = model.Remarks?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var bonus = await _repository.GetByIdAsync(model.Id, companyId);
            if (bonus == null) return (false, "Bonus record not found.");
            bonus.BonusAmount = model.BonusAmount;
            bonus.Remarks = model.Remarks?.Trim();
            bonus.UpdatedAt = DateTime.UtcNow;
            bonus.UpdatedBy = _currentUserService.UserName;
        }
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> GenerateBulkAsync(int year, int bonusType, decimal percentage)
    {
        var companyId = _currentUserService.CompanyId;
        var existing = await _repository.GetByYearAsync(companyId, year);
        var existingEmpIds = existing.Where(b => (int)b.BonusType == bonusType).Select(b => b.EmployeeId).ToHashSet();

        var employees = await _context.Employees.Where(e => e.CompanyId == companyId).ToListAsync();
        var newBonuses = employees
            .Where(e => !existingEmpIds.Contains(e.Id))
            .Select(e => new EidBonus
            {
                CompanyId = companyId,
                EmployeeId = e.Id,
                BonusType = (BonusType)bonusType,
                Year = year,
                BonusAmount = Math.Round(e.GrossSalary * percentage / 100m, 2),
                BonusStatus = BonusStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            }).ToList();

        if (newBonuses.Count == 0) return (false, "All employees already have bonuses for this period.");
        await _repository.AddRangeAsync(newBonuses);
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ApproveAsync(int id)
    {
        var bonus = await _repository.GetByIdAsync(id, _currentUserService.CompanyId);
        if (bonus == null) return (false, "Bonus not found.");
        bonus.BonusStatus = BonusStatus.Approved;
        bonus.UpdatedAt = DateTime.UtcNow;
        bonus.UpdatedBy = _currentUserService.UserName;
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var bonus = await _repository.GetByIdAsync(id, _currentUserService.CompanyId);
        if (bonus == null) return (false, "Bonus not found.");
        bonus.IsDeleted = true;
        bonus.UpdatedAt = DateTime.UtcNow;
        bonus.UpdatedBy = _currentUserService.UserName;
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    private static EidBonusVm MapVm(EidBonus b) => new(
        b.Id, b.EmployeeId,
        b.Employee?.FullName ?? string.Empty,
        b.Employee?.EmployeeCode ?? string.Empty,
        b.Employee?.Department?.Name ?? "-",
        (int)b.BonusType, b.BonusType.ToString(),
        b.Year, b.BonusAmount,
        (int)b.BonusStatus, b.BonusStatus.ToString(),
        b.Remarks);
}

public class JobCardService : IJobCardService
{
    private readonly IJobCardRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public JobCardService(IJobCardRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<JobCardVm>> GetByDateRangeAsync(DateTime from, DateTime to, int? lineId)
    {
        var list = await _repository.GetByDateRangeAsync(_currentUserService.CompanyId, from, to);
        if (lineId.HasValue)
            list = list.Where(j => j.LineId == lineId).ToList();
        return list.Select(MapVm).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveAsync(JobCardCreateVm model)
    {
        var companyId = _currentUserService.CompanyId;
        if (model.Id == 0)
        {
            await _repository.AddAsync(new JobCard
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                LineId = model.LineId,
                WorkDate = model.WorkDate.Date,
                TaskDescription = model.TaskDescription?.Trim(),
                TargetQty = model.TargetQty,
                AchievedQty = model.AchievedQty,
                Remarks = model.Remarks?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var existing = await _repository.GetByIdAsync(model.Id, companyId);
            if (existing == null) return (false, "Job card not found.");
            existing.EmployeeId = model.EmployeeId;
            existing.LineId = model.LineId;
            existing.WorkDate = model.WorkDate.Date;
            existing.TaskDescription = model.TaskDescription?.Trim();
            existing.TargetQty = model.TargetQty;
            existing.AchievedQty = model.AchievedQty;
            existing.Remarks = model.Remarks?.Trim();
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUserService.UserName;
        }
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var card = await _repository.GetByIdAsync(id, _currentUserService.CompanyId);
        if (card == null) return (false, "Job card not found.");
        card.IsDeleted = true;
        card.UpdatedAt = DateTime.UtcNow;
        card.UpdatedBy = _currentUserService.UserName;
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    private static JobCardVm MapVm(JobCard j) => new(
        j.Id, j.EmployeeId,
        j.Employee?.FullName ?? string.Empty,
        j.Employee?.EmployeeCode ?? string.Empty,
        j.Line?.Name ?? "-",
        j.Employee?.Department?.Name ?? "-",
        j.WorkDate,
        j.TaskDescription,
        j.TargetQty, j.AchievedQty,
        j.TargetQty > 0 ? Math.Round(j.AchievedQty / j.TargetQty * 100m, 1) : 0m,
        j.Remarks);
}

public class MonthlyReportService : IMonthlyReportService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MonthlyReportService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<MonthlyReportVm> GenerateAsync(int year, int month)
    {
        var companyId = _currentUserService.CompanyId;
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var monthName = from.ToString("MMMM yyyy");

        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        var attendances = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate >= from && a.AttendanceDate <= to)
            .ToListAsync();

        var leaves = await _context.LeaveApplications
            .Where(l => l.CompanyId == companyId && l.ApprovalStatus == LeaveApprovalStatus.Approved
                && l.FromDate <= to && l.ToDate >= from)
            .ToListAsync();

        var holidays = await _context.Holidays
            .Where(h => h.CompanyId == companyId && h.HolidayDate >= from && h.HolidayDate <= to)
            .CountAsync();

        var payrollSheets = await _context.PayrollSheets
            .Include(s => s.PayrollPeriod)
            .Where(s => s.CompanyId == companyId && s.PayrollPeriod!.StartDate.Year == year && s.PayrollPeriod.StartDate.Month == month)
            .ToListAsync();

        var payrollDetails = payrollSheets.Count > 0
            ? await _context.PayrollDetails
                .Include(d => d.Employee).ThenInclude(e => e!.Department)
                .Where(d => d.CompanyId == companyId && payrollSheets.Select(s => s.Id).Contains(d.PayrollSheetId))
                .ToListAsync()
            : new List<PayrollDetail>();

        var bonuses = await _context.EidBonuses
            .Where(b => b.CompanyId == companyId && b.Year == year)
            .SumAsync(b => b.BonusAmount);

        var advances = await _context.AdvanceSalaries
            .Where(a => a.CompanyId == companyId && a.ApprovedDate.HasValue
                && a.ApprovedDate.Value.Year == year && a.ApprovedDate.Value.Month == month)
            .SumAsync(a => a.Amount);

        var byDept = employees
            .GroupBy(e => e.Department?.Name ?? "Unassigned")
            .Select(g =>
            {
                var empIds = g.Select(e => e.Id).ToHashSet();
                var att = attendances.Where(a => empIds.Contains(a.EmployeeId)).ToList();
                var details = payrollDetails.Where(d => empIds.Contains(d.EmployeeId)).ToList();
                return new MonthlyReportDeptVm(
                    g.Key,
                    g.Count(),
                    att.Count(a => !a.IsAbsent && a.InTime.HasValue),
                    att.Count(a => a.IsAbsent),
                    details.Sum(d => d.GrossSalary),
                    details.Sum(d => d.NetPayableSalary),
                    Math.Round(att.Sum(a => a.OvertimeMinutes) / 60m, 2));
            })
            .OrderBy(d => d.Department)
            .ToList();

        return new MonthlyReportVm(
            year, month, monthName,
            employees.Count,
            attendances.Count(a => !a.IsAbsent && a.InTime.HasValue),
            attendances.Count(a => a.IsAbsent),
            leaves.Count,
            holidays,
            Math.Round(attendances.Sum(a => a.OvertimeMinutes) / 60m, 2),
            payrollDetails.Sum(d => d.NetPayableSalary),
            bonuses,
            advances,
            byDept);
    }
}
