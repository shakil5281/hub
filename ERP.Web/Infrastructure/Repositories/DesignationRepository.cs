using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class DesignationRepository : Repository<Designation>, IDesignationRepository
{
    public DesignationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Designation>> GetByCompanyAsync(int companyId)
        => await DbSet
            .Include(d => d.Section)
            .ThenInclude(s => s!.Department)
            .Where(d => d.CompanyId == companyId)
            .OrderBy(d => d.Title)
            .ToListAsync();

    public async Task<IReadOnlyList<Designation>> GetBySectionAsync(int sectionId, int companyId)
        => await DbSet
            .Include(d => d.Section)
            .ThenInclude(s => s!.Department)
            .Where(d => d.CompanyId == companyId && d.SectionId == sectionId)
            .OrderBy(d => d.Title)
            .ToListAsync();
}
