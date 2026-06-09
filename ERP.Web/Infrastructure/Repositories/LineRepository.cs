using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class LineRepository : Repository<Line>, ILineRepository
{
    public LineRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Line>> GetBySectionAsync(int sectionId, int companyId)
        => await DbSet
            .Include(l => l.Section)
            .Where(l => l.SectionId == sectionId && l.CompanyId == companyId)
            .OrderBy(l => l.Name)
            .ToListAsync();

    public async Task<IReadOnlyList<Line>> GetByCompanyAsync(int companyId)
        => await DbSet
            .Include(l => l.Section)
            .ThenInclude(s => s!.Department)
            .Where(l => l.CompanyId == companyId)
            .OrderBy(l => l.Name)
            .ToListAsync();
}
