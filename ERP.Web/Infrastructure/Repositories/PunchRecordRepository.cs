using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class PunchRecordRepository : Repository<PunchRecord>, IPunchRecordRepository
{
    public PunchRecordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PunchRecord>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to)
        => await DbSet
            .Include(p => p.Employee)
            .Where(p => p.CompanyId == companyId && p.PunchTime >= from && p.PunchTime <= to)
            .OrderBy(p => p.PunchTime)
            .ToListAsync();
}
