using ERP.Web.Core.DTOs;
using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IReadOnlyList<Employee>> GetByCompanyAsync(int companyId);
    Task<IReadOnlyList<Employee>> GetFilteredAsync(int companyId, EmployeeFilterDto filter);
    Task<Employee?> GetByIdWithDetailsAsync(int id, int companyId);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, int companyId, int? excludeId = null);
    Task<bool> PunchNumberExistsAsync(string punchNumber, int companyId, int? excludeId = null);
    Task<int> CountByCompanyAsync(int companyId);
}
