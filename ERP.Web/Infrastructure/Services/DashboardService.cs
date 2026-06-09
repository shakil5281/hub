using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using ERP.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDailyAttendanceRepository _dailyAttendanceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly AppDbContext _context;

    public DashboardService(
        IEmployeeRepository employeeRepository,
        IDailyAttendanceRepository dailyAttendanceRepository,
        ICurrentUserService currentUserService,
        AppDbContext context)
    {
        _employeeRepository = employeeRepository;
        _dailyAttendanceRepository = dailyAttendanceRepository;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var companyId = _currentUserService.CompanyId;
        var today = DateTime.Today;

        var totalEmployees = await _employeeRepository.CountByCompanyAsync(companyId);
        var presentToday = await _dailyAttendanceRepository.CountPresentTodayAsync(companyId, today);
        var absentToday = await _dailyAttendanceRepository.CountAbsentTodayAsync(companyId, today);

        var onLeaveToday = await _context.LeaveApplications.CountAsync(l =>
            l.CompanyId == companyId &&
            l.ApprovalStatus == Core.Enums.LeaveApprovalStatus.Approved &&
            l.FromDate.Date <= today &&
            l.ToDate.Date >= today);

        var pendingLeave = await _context.LeaveApplications.CountAsync(l =>
            l.CompanyId == companyId &&
            l.ApprovalStatus == Core.Enums.LeaveApprovalStatus.Pending);

        var pendingAttendance = await _context.DailyAttendances.CountAsync(a =>
            a.CompanyId == companyId &&
            a.AttendanceDate == today &&
            !a.IsApproved &&
            !a.IsAbsent);

        var payrollData = await _context.PayrollSheets
            .Where(p => p.CompanyId == companyId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(6)
            .Select(p => new { p.CreatedAt, p.TotalNetPayable })
            .ToListAsync();

        return new DashboardViewModel
        {
            TotalEmployees = totalEmployees,
            PresentToday = presentToday,
            AbsentToday = absentToday,
            OnLeaveToday = onLeaveToday,
            PendingLeaveApplications = pendingLeave,
            PendingAttendanceApprovals = pendingAttendance,
            PayrollChartLabels = payrollData.Select(p => p.CreatedAt.ToString("MMM yyyy")).ToList(),
            PayrollChartAmounts = payrollData.Select(p => (double)p.TotalNetPayable).ToList()
        };
    }
}
