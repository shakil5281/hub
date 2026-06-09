using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IReadOnlyList<Department>> GetByCompanyAsync(int companyId);
}
