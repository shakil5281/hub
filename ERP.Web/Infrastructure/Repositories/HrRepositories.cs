using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class StaffingPlanRepository : IStaffingPlanRepository
{
    private readonly AppDbContext _context;
    public StaffingPlanRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<StaffingPlan>> GetByPeriodAsync(int companyId, int year, int month)
        => await _context.StaffingPlans
            .Include(s => s.Department).Include(s => s.Section).Include(s => s.Line).Include(s => s.Designation)
            .Where(s => s.CompanyId == companyId && s.Year == year && s.Month == month)
            .OrderBy(s => s.Department!.Name).ThenBy(s => s.Section!.Name)
            .ToListAsync();

    public async Task<StaffingPlan?> GetByIdAsync(int id, int companyId)
        => await _context.StaffingPlans
            .Include(s => s.Department).Include(s => s.Section).Include(s => s.Line).Include(s => s.Designation)
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);

    public async Task<StaffingPlan?> GetDuplicateAsync(int companyId, int departmentId, int? sectionId, int? lineId, int? designationId, int year, int month, int? excludeId = null)
        => await _context.StaffingPlans.FirstOrDefaultAsync(s =>
            s.CompanyId == companyId &&
            s.DepartmentId == departmentId &&
            s.SectionId == sectionId &&
            s.LineId == lineId &&
            s.DesignationId == designationId &&
            s.Year == year &&
            s.Month == month &&
            (!excludeId.HasValue || s.Id != excludeId.Value));

    public async Task AddAsync(StaffingPlan plan) => await _context.StaffingPlans.AddAsync(plan);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class HiringRequestRepository : IHiringRequestRepository
{
    private readonly AppDbContext _context;
    public HiringRequestRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<HiringRequest>> GetAllAsync(int companyId, HiringRequestStatus? status = null)
    {
        var query = _context.HiringRequests
            .Include(h => h.Department).Include(h => h.Section).Include(h => h.Line).Include(h => h.Designation)
            .Where(h => h.CompanyId == companyId);
        if (status.HasValue) query = query.Where(h => h.HiringRequestStatus == status.Value);
        return await query.OrderByDescending(h => h.RequestDate).ToListAsync();
    }

    public async Task<HiringRequest?> GetByIdAsync(int id, int companyId)
        => await _context.HiringRequests
            .Include(h => h.Department).Include(h => h.Section).Include(h => h.Line).Include(h => h.Designation)
            .FirstOrDefaultAsync(h => h.Id == id && h.CompanyId == companyId);

    public async Task<IReadOnlyList<HiringRequest>> GetApprovedForPeriodAsync(int companyId, int year, int month)
        => await _context.HiringRequests
            .Where(h => h.CompanyId == companyId &&
                h.HiringRequestStatus == HiringRequestStatus.Approved &&
                h.RequestDate.Year == year &&
                h.RequestDate.Month == month)
            .ToListAsync();

    public async Task AddAsync(HiringRequest request) => await _context.HiringRequests.AddAsync(request);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class EmployeeSeparationRepository : IEmployeeSeparationRepository
{
    private readonly AppDbContext _context;
    public EmployeeSeparationRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<EmployeeSeparation>> GetFilteredAsync(int companyId, DateTime? from, DateTime? to, SeparationType? type, int? departmentId)
    {
        var query = _context.EmployeeSeparations
            .Include(s => s.Employee).ThenInclude(e => e!.Department)
            .Where(s => s.CompanyId == companyId);
        if (from.HasValue) query = query.Where(s => s.SeparationDate >= from.Value.Date);
        if (to.HasValue) query = query.Where(s => s.SeparationDate <= to.Value.Date);
        if (type.HasValue) query = query.Where(s => s.SeparationType == type.Value);
        if (departmentId.HasValue) query = query.Where(s => s.Employee!.DepartmentId == departmentId.Value);
        return await query.OrderByDescending(s => s.SeparationDate).ToListAsync();
    }

    public async Task<EmployeeSeparation?> GetByIdAsync(int id, int companyId)
        => await _context.EmployeeSeparations
            .Include(s => s.Employee).ThenInclude(e => e!.Department)
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);

    public async Task<EmployeeSeparation?> GetByEmployeeIdAsync(int companyId, int employeeId)
        => await _context.EmployeeSeparations
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.EmployeeId == employeeId);

    public async Task AddAsync(EmployeeSeparation separation) => await _context.EmployeeSeparations.AddAsync(separation);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
