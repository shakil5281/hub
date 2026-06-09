using ERP.Web.Core.DTOs;
using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Employee>> GetFilteredAsync(int companyId, EmployeeFilterDto filter)
    {
        var query = DbSet
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Line)
            .Include(e => e.Designation)
            .Where(e => e.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(e =>
                e.FullName.ToLower().Contains(term) ||
                e.EmployeeCode.ToLower().Contains(term) ||
                e.PunchNumber.ToLower().Contains(term));
        }
        if (filter.DepartmentId.HasValue) query = query.Where(e => e.DepartmentId == filter.DepartmentId);
        if (filter.SectionId.HasValue) query = query.Where(e => e.SectionId == filter.SectionId);
        if (filter.LineId.HasValue) query = query.Where(e => e.LineId == filter.LineId);
        if (filter.DesignationId.HasValue) query = query.Where(e => e.DesignationId == filter.DesignationId);
        if (filter.Status.HasValue) query = query.Where(e => e.Status == filter.Status);
        if (filter.JoiningFrom.HasValue) query = query.Where(e => e.JoiningDate >= filter.JoiningFrom.Value.Date);
        if (filter.JoiningTo.HasValue) query = query.Where(e => e.JoiningDate <= filter.JoiningTo.Value.Date);
        if (filter.MinSalary.HasValue) query = query.Where(e => e.GrossSalary >= filter.MinSalary);
        if (filter.MaxSalary.HasValue) query = query.Where(e => e.GrossSalary <= filter.MaxSalary);

        return await query.OrderBy(e => e.EmployeeCode).ToListAsync();
    }

    public async Task<IReadOnlyList<Employee>> GetByCompanyAsync(int companyId)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Line)
            .Include(e => e.Designation)
            .Where(e => e.CompanyId == companyId)
            .OrderBy(e => e.EmployeeCode)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id, int companyId)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Line)
            .Include(e => e.Designation)
            .Include(e => e.Address)
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId);
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, int companyId, int? excludeId = null)
    {
        return await DbSet.AnyAsync(e =>
            e.CompanyId == companyId &&
            e.EmployeeCode == employeeCode &&
            (!excludeId.HasValue || e.Id != excludeId.Value));
    }

    public async Task<bool> PunchNumberExistsAsync(string punchNumber, int companyId, int? excludeId = null)
    {
        return await DbSet.AnyAsync(e =>
            e.CompanyId == companyId &&
            e.PunchNumber == punchNumber &&
            (!excludeId.HasValue || e.Id != excludeId.Value));
    }

    public async Task<int> CountByCompanyAsync(int companyId)
        => await DbSet.CountAsync(e => e.CompanyId == companyId);
}
