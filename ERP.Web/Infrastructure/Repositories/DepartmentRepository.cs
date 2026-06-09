using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Department>> GetByCompanyAsync(int companyId)
        => await DbSet.Where(d => d.CompanyId == companyId).OrderBy(d => d.Name).ToListAsync();
}
