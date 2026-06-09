using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class SectionRepository : Repository<Section>, ISectionRepository
{
    public SectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Section>> GetByDepartmentAsync(int departmentId, int companyId)
        => await DbSet
            .Include(s => s.Department)
            .Where(s => s.DepartmentId == departmentId && s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public async Task<IReadOnlyList<Section>> GetByCompanyAsync(int companyId)
        => await DbSet
            .Include(s => s.Department)
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .ToListAsync();
}
