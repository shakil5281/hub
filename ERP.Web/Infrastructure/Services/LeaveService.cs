using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public LeaveService(
        ILeaveRepository leaveRepository,
        IHolidayRepository holidayRepository,
        AppDbContext context,
        ICurrentUserService currentUserService)
    {
        _leaveRepository = leaveRepository;
        _holidayRepository = holidayRepository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<LeaveTypeVm>> GetLeaveTypesAsync()
    {
        var types = await _leaveRepository.GetLeaveTypesAsync(_currentUserService.CompanyId);
        return types.Select(t => new LeaveTypeVm(t.Id, t.Name, t.MaxDaysPerYear, t.IsPaid)).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveLeaveTypeAsync(LeaveTypeVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            await _leaveRepository.AddLeaveTypeAsync(new LeaveType
            {
                CompanyId = companyId,
                Name = model.Name.Trim(),
                MaxDaysPerYear = model.MaxDaysPerYear,
                IsPaid = model.IsPaid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(t => t.Id == model.Id && t.CompanyId == companyId);
            if (leaveType == null) return (false, "Leave type not found.");

            leaveType.Name = model.Name.Trim();
            leaveType.MaxDaysPerYear = model.MaxDaysPerYear;
            leaveType.IsPaid = model.IsPaid;
            leaveType.UpdatedAt = DateTime.UtcNow;
            leaveType.UpdatedBy = _currentUserService.UserName;
        }

        await _leaveRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<LeaveBalanceVm>> GetBalancesAsync(int? employeeId)
    {
        var balances = await _leaveRepository.GetBalancesAsync(_currentUserService.CompanyId, employeeId);
        return balances.Select(b => new LeaveBalanceVm(
            b.Id,
            b.EmployeeId,
            b.Employee?.FullName ?? string.Empty,
            b.LeaveTypeId,
            b.LeaveType?.Name ?? string.Empty,
            b.Year,
            b.AllocatedDays,
            b.UsedDays,
            b.RemainingDays)).ToList();
    }

    public async Task<(bool Success, string? Error)> AdjustBalanceAsync(LeaveBalanceVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            _context.LeaveBalances.Add(new LeaveBalance
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId,
                Year = model.Year,
                AllocatedDays = model.AllocatedDays,
                UsedDays = model.UsedDays,
                RemainingDays = model.AllocatedDays - model.UsedDays,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.Id == model.Id && b.CompanyId == companyId);
            if (balance == null) return (false, "Leave balance not found.");

            balance.AllocatedDays = model.AllocatedDays;
            balance.UsedDays = model.UsedDays;
            balance.RemainingDays = model.AllocatedDays - model.UsedDays;
            balance.UpdatedAt = DateTime.UtcNow;
            balance.UpdatedBy = _currentUserService.UserName;
        }

        await _leaveRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<LeaveApplicationVm>> GetApplicationsAsync()
    {
        var applications = await _leaveRepository.GetApplicationsAsync(_currentUserService.CompanyId);
        return applications.Select(a => new LeaveApplicationVm(
            a.Id,
            a.Employee?.FullName ?? string.Empty,
            a.LeaveType?.Name ?? string.Empty,
            a.FromDate,
            a.ToDate,
            a.TotalDays,
            a.ApprovalStatus.ToString(),
            a.Reason)).ToList();
    }

    public async Task<(bool Success, string? Error)> SubmitApplicationAsync(LeaveApplicationCreateVm model)
    {
        if (model.ToDate.Date < model.FromDate.Date)
            return (false, "To date cannot be before from date.");

        var totalDays = (model.ToDate.Date - model.FromDate.Date).Days + 1;

        await _leaveRepository.AddApplicationAsync(new LeaveApplication
        {
            CompanyId = _currentUserService.CompanyId,
            EmployeeId = model.EmployeeId,
            LeaveTypeId = model.LeaveTypeId,
            FromDate = model.FromDate.Date,
            ToDate = model.ToDate.Date,
            TotalDays = totalDays,
            Reason = model.Reason?.Trim(),
            ApprovalStatus = LeaveApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserName
        });

        await _leaveRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ApproveAsync(int id, bool approved)
    {
        var application = await _leaveRepository.GetApplicationByIdAsync(id, _currentUserService.CompanyId);
        if (application == null) return (false, "Leave application not found.");
        if (application.ApprovalStatus != LeaveApprovalStatus.Pending)
            return (false, "Application has already been processed.");

        application.ApprovalStatus = approved ? LeaveApprovalStatus.Approved : LeaveApprovalStatus.Rejected;
        application.UpdatedAt = DateTime.UtcNow;
        application.UpdatedBy = _currentUserService.UserName;

        if (approved)
        {
            var year = application.FromDate.Year;
            var balance = await _context.LeaveBalances.FirstOrDefaultAsync(b =>
                b.CompanyId == _currentUserService.CompanyId
                && b.EmployeeId == application.EmployeeId
                && b.LeaveTypeId == application.LeaveTypeId
                && b.Year == year);

            if (balance == null)
            {
                balance = new LeaveBalance
                {
                    CompanyId = _currentUserService.CompanyId,
                    EmployeeId = application.EmployeeId,
                    LeaveTypeId = application.LeaveTypeId,
                    Year = year,
                    AllocatedDays = 0,
                    UsedDays = application.TotalDays,
                    RemainingDays = -application.TotalDays,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _currentUserService.UserName
                };
                _context.LeaveBalances.Add(balance);
            }
            else
            {
                balance.UsedDays += application.TotalDays;
                balance.RemainingDays = balance.AllocatedDays - balance.UsedDays;
                balance.UpdatedAt = DateTime.UtcNow;
                balance.UpdatedBy = _currentUserService.UserName;
            }
        }

        await _leaveRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<HolidayVm>> GetHolidaysAsync(int year)
    {
        var holidays = await _holidayRepository.GetByYearAsync(_currentUserService.CompanyId, year);
        return holidays.Select(h => new HolidayVm(h.Id, h.Name, h.HolidayDate, h.HolidayType.ToString(), (int)h.HolidayType, h.Description)).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveHolidayAsync(HolidayCreateVm model)
    {
        var companyId = _currentUserService.CompanyId;
        if (model.Id == 0)
        {
            await _holidayRepository.AddAsync(new Holiday
            {
                CompanyId = companyId,
                Name = model.Name.Trim(),
                HolidayDate = model.HolidayDate.Date,
                HolidayType = (HolidayType)model.HolidayType,
                Description = model.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var holiday = await _holidayRepository.GetByIdAsync(model.Id, companyId);
            if (holiday == null) return (false, "Holiday not found.");
            holiday.Name = model.Name.Trim();
            holiday.HolidayDate = model.HolidayDate.Date;
            holiday.HolidayType = (HolidayType)model.HolidayType;
            holiday.Description = model.Description?.Trim();
            holiday.UpdatedAt = DateTime.UtcNow;
            holiday.UpdatedBy = _currentUserService.UserName;
        }
        await _holidayRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteHolidayAsync(int id)
    {
        var companyId = _currentUserService.CompanyId;
        var holiday = await _holidayRepository.GetByIdAsync(id, companyId);
        if (holiday == null) return (false, "Holiday not found.");
        holiday.IsDeleted = true;
        holiday.UpdatedAt = DateTime.UtcNow;
        holiday.UpdatedBy = _currentUserService.UserName;
        await _holidayRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<MonthlyLeaveRecordVm>> GetMonthlyLeaveRecordAsync(int year, int month)
    {
        var companyId = _currentUserService.CompanyId;
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        var applications = await _context.LeaveApplications
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Include(a => a.LeaveType)
            .Where(a => a.CompanyId == companyId && a.FromDate <= to && a.ToDate >= from)
            .OrderBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        return applications
            .GroupBy(a => new { a.EmployeeId, Code = a.Employee!.EmployeeCode, Name = a.Employee.FullName, Dept = a.Employee.Department?.Name ?? "-" })
            .Select(g => new MonthlyLeaveRecordVm(
                g.Key.Code,
                g.Key.Name,
                g.Key.Dept,
                g.Count(),
                g.Count(x => x.ApprovalStatus == LeaveApprovalStatus.Approved),
                g.Count(x => x.ApprovalStatus == LeaveApprovalStatus.Pending),
                g.Count(x => x.ApprovalStatus == LeaveApprovalStatus.Rejected),
                g.Select(x => new LeaveDetailRowVm(
                    x.LeaveType?.Name ?? "-",
                    x.FromDate, x.ToDate, x.TotalDays, x.ApprovalStatus.ToString())).ToList()))
            .ToList();
    }

    public async Task<IReadOnlyList<EarnLeaveVm>> GetEarnLeaveReportAsync(int year)
    {
        var companyId = _currentUserService.CompanyId;
        var earnLeaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(t => t.CompanyId == companyId && t.Name.ToLower().Contains("earn"));

        var query = _context.LeaveBalances
            .Include(b => b.Employee).ThenInclude(e => e!.Department)
            .Where(b => b.CompanyId == companyId && b.Year == year);

        if (earnLeaveType != null)
            query = query.Where(b => b.LeaveTypeId == earnLeaveType.Id);

        var balances = await query.OrderBy(b => b.Employee!.EmployeeCode).ToListAsync();

        return balances.Select(b => new EarnLeaveVm(
            b.Employee!.EmployeeCode,
            b.Employee.FullName,
            b.Employee.Department?.Name ?? "-",
            b.AllocatedDays,
            b.UsedDays,
            b.RemainingDays,
            b.Year)).ToList();
    }
}
