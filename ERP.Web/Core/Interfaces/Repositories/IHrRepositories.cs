using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface IStaffingPlanRepository
{
    Task<IReadOnlyList<StaffingPlan>> GetByPeriodAsync(int companyId, int year, int month);
    Task<StaffingPlan?> GetByIdAsync(int id, int companyId);
    Task<StaffingPlan?> GetDuplicateAsync(int companyId, int departmentId, int? sectionId, int? lineId, int? designationId, int year, int month, int? excludeId = null);
    Task AddAsync(StaffingPlan plan);
    Task SaveChangesAsync();
}

public interface IHiringRequestRepository
{
    Task<IReadOnlyList<HiringRequest>> GetAllAsync(int companyId, HiringRequestStatus? status = null);
    Task<HiringRequest?> GetByIdAsync(int id, int companyId);
    Task<IReadOnlyList<HiringRequest>> GetApprovedForPeriodAsync(int companyId, int year, int month);
    Task AddAsync(HiringRequest request);
    Task SaveChangesAsync();
}

public interface IEmployeeSeparationRepository
{
    Task<IReadOnlyList<EmployeeSeparation>> GetFilteredAsync(int companyId, DateTime? from, DateTime? to, SeparationType? type, int? departmentId);
    Task<EmployeeSeparation?> GetByIdAsync(int id, int companyId);
    Task<EmployeeSeparation?> GetByEmployeeIdAsync(int companyId, int employeeId);
    Task AddAsync(EmployeeSeparation separation);
    Task SaveChangesAsync();
}
