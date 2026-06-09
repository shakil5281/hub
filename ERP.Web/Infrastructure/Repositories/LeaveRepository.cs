using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _context;

    public LeaveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<LeaveType>> GetLeaveTypesAsync(int companyId)
        => await _context.LeaveTypes
            .Where(t => t.CompanyId == companyId)
            .OrderBy(t => t.Name)
            .ToListAsync();

    public async Task<IReadOnlyList<LeaveApplication>> GetApplicationsAsync(int companyId)
        => await _context.LeaveApplications
            .Include(a => a.Employee)
            .Include(a => a.LeaveType)
            .Where(a => a.CompanyId == companyId)
            .OrderByDescending(a => a.FromDate)
            .ToListAsync();

    public async Task<IReadOnlyList<LeaveBalance>> GetBalancesAsync(int companyId, int? employeeId = null)
    {
        var query = _context.LeaveBalances
            .Include(b => b.Employee)
            .Include(b => b.LeaveType)
            .Where(b => b.CompanyId == companyId);

        if (employeeId.HasValue)
            query = query.Where(b => b.EmployeeId == employeeId.Value);

        return await query.OrderBy(b => b.Employee!.FullName).ThenBy(b => b.LeaveType!.Name).ToListAsync();
    }

    public async Task AddLeaveTypeAsync(LeaveType entity)
        => await _context.LeaveTypes.AddAsync(entity);

    public async Task AddApplicationAsync(LeaveApplication entity)
        => await _context.LeaveApplications.AddAsync(entity);

    public async Task<LeaveApplication?> GetApplicationByIdAsync(int id, int companyId)
        => await _context.LeaveApplications
            .Include(a => a.Employee)
            .Include(a => a.LeaveType)
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == companyId);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
