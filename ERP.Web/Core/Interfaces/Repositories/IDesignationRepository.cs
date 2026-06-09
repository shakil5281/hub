using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface IDesignationRepository : IRepository<Designation>
{
    Task<IReadOnlyList<Designation>> GetByCompanyAsync(int companyId);
    Task<IReadOnlyList<Designation>> GetBySectionAsync(int sectionId, int companyId);
}
