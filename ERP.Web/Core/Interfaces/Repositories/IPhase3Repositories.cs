using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface ITemporaryShiftRepository
{
    Task<IReadOnlyList<TemporaryShiftAssignment>> GetByDateAsync(int companyId, DateTime date);
    Task<IReadOnlyList<TemporaryShiftAssignment>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<TemporaryShiftAssignment>> GetByEmployeeAsync(int companyId, int employeeId);
    Task<TemporaryShiftAssignment?> GetByEmployeeDateAsync(int companyId, int employeeId, DateTime date);
    Task AddAsync(TemporaryShiftAssignment assignment);
    Task<TemporaryShiftAssignment?> GetByIdAsync(int id, int companyId);
    Task SaveChangesAsync();
}

public interface IHolidayRepository
{
    Task<IReadOnlyList<Holiday>> GetByYearAsync(int companyId, int year);
    Task<IReadOnlyList<Holiday>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to);
    Task<bool> IsHolidayAsync(int companyId, DateTime date);
    Task<Holiday?> GetByIdAsync(int id, int companyId);
    Task AddAsync(Holiday holiday);
    Task SaveChangesAsync();
}

public interface IEidBonusRepository
{
    Task<IReadOnlyList<EidBonus>> GetByYearAsync(int companyId, int year);
    Task<IReadOnlyList<EidBonus>> GetByEmployeeAsync(int companyId, int employeeId);
    Task<EidBonus?> GetByIdAsync(int id, int companyId);
    Task AddAsync(EidBonus bonus);
    Task AddRangeAsync(IEnumerable<EidBonus> bonuses);
    Task SaveChangesAsync();
}

public interface IJobCardRepository
{
    Task<IReadOnlyList<JobCard>> GetByDateAsync(int companyId, DateTime date);
    Task<IReadOnlyList<JobCard>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<JobCard>> GetByLineAsync(int companyId, int lineId, DateTime date);
    Task<IReadOnlyList<JobCard>> GetByEmployeeAsync(int companyId, int employeeId);
    Task<JobCard?> GetByIdAsync(int id, int companyId);
    Task AddAsync(JobCard jobCard);
    Task SaveChangesAsync();
}
