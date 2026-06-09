using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.DTOs;

namespace ERP.Web.Core.Interfaces.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeListItemVm>> GetAllAsync();
    Task<IReadOnlyList<EmployeeListItemVm>> SearchAsync(EmployeeFilterDto filter);
    Task<EmployeeDetailsVm?> GetDetailsAsync(int id);
    Task<EmployeeEditVm?> GetForEditAsync(int id);
    Task<(bool Success, string? Error)> CreateAsync(EmployeeCreateVm model);
    Task<(bool Success, string? Error)> UpdateAsync(EmployeeEditVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
    Task<EmployeeFormLookupsVm> GetFormLookupsAsync();
}
