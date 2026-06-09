using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface ISectionRepository : IRepository<Section>
{
    Task<IReadOnlyList<Section>> GetByDepartmentAsync(int departmentId, int companyId);
    Task<IReadOnlyList<Section>> GetByCompanyAsync(int companyId);
}

public interface ILineRepository : IRepository<Line>
{
    Task<IReadOnlyList<Line>> GetBySectionAsync(int sectionId, int companyId);
    Task<IReadOnlyList<Line>> GetByCompanyAsync(int companyId);
}

public interface IShiftRepository : IRepository<Shift>
{
    Task<IReadOnlyList<Shift>> GetByCompanyAsync(int companyId);
}

public interface IPunchRecordRepository : IRepository<PunchRecord>
{
    Task<IReadOnlyList<PunchRecord>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to);
}

public interface ILeaveRepository
{
    Task<IReadOnlyList<LeaveType>> GetLeaveTypesAsync(int companyId);
    Task<IReadOnlyList<LeaveApplication>> GetApplicationsAsync(int companyId);
    Task<IReadOnlyList<LeaveBalance>> GetBalancesAsync(int companyId, int? employeeId = null);
    Task AddLeaveTypeAsync(LeaveType entity);
    Task AddApplicationAsync(LeaveApplication entity);
    Task<LeaveApplication?> GetApplicationByIdAsync(int id, int companyId);
    Task SaveChangesAsync();
}

public interface IPayrollRepository
{
    Task<IReadOnlyList<PayrollPeriod>> GetPeriodsAsync(int companyId);
    Task<PayrollPeriod?> GetPeriodByIdAsync(int id, int companyId);
    Task<PayrollSheet?> GetSheetByIdAsync(int id, int companyId);
    Task<IReadOnlyList<PayrollDetail>> GetDetailsBySheetAsync(int sheetId, int companyId);
    Task AddPeriodAsync(PayrollPeriod period);
    Task AddSheetAsync(PayrollSheet sheet);
    Task AddDetailAsync(PayrollDetail detail);
    Task SaveChangesAsync();
}
