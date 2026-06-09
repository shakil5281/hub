using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class TemporaryShiftRepository : ITemporaryShiftRepository
{
    private readonly AppDbContext _context;
    public TemporaryShiftRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<TemporaryShiftAssignment>> GetByDateAsync(int companyId, DateTime date)
        => await _context.TemporaryShiftAssignments
            .Include(t => t.Employee).Include(t => t.Shift)
            .Where(t => t.CompanyId == companyId && t.AssignmentDate.Date == date.Date)
            .ToListAsync();

    public async Task<IReadOnlyList<TemporaryShiftAssignment>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to)
        => await _context.TemporaryShiftAssignments
            .Include(t => t.Employee).ThenInclude(e => e!.Department)
            .Include(t => t.Shift)
            .Where(t => t.CompanyId == companyId && t.AssignmentDate.Date >= from.Date && t.AssignmentDate.Date <= to.Date)
            .OrderByDescending(t => t.AssignmentDate)
            .ToListAsync();

    public async Task<IReadOnlyList<TemporaryShiftAssignment>> GetByEmployeeAsync(int companyId, int employeeId)
        => await _context.TemporaryShiftAssignments
            .Include(t => t.Shift)
            .Where(t => t.CompanyId == companyId && t.EmployeeId == employeeId)
            .OrderByDescending(t => t.AssignmentDate)
            .ToListAsync();

    public async Task<TemporaryShiftAssignment?> GetByEmployeeDateAsync(int companyId, int employeeId, DateTime date)
        => await _context.TemporaryShiftAssignments
            .Include(t => t.Shift)
            .FirstOrDefaultAsync(t => t.CompanyId == companyId && t.EmployeeId == employeeId && t.AssignmentDate.Date == date.Date);

    public async Task<TemporaryShiftAssignment?> GetByIdAsync(int id, int companyId)
        => await _context.TemporaryShiftAssignments
            .Include(t => t.Employee).Include(t => t.Shift)
            .FirstOrDefaultAsync(t => t.Id == id && t.CompanyId == companyId);

    public async Task AddAsync(TemporaryShiftAssignment assignment)
        => await _context.TemporaryShiftAssignments.AddAsync(assignment);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class HolidayRepository : IHolidayRepository
{
    private readonly AppDbContext _context;
    public HolidayRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Holiday>> GetByYearAsync(int companyId, int year)
        => await _context.Holidays
            .Where(h => h.CompanyId == companyId && h.HolidayDate.Year == year)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();

    public async Task<IReadOnlyList<Holiday>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to)
        => await _context.Holidays
            .Where(h => h.CompanyId == companyId && h.HolidayDate.Date >= from.Date && h.HolidayDate.Date <= to.Date)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();

    public async Task<bool> IsHolidayAsync(int companyId, DateTime date)
        => await _context.Holidays
            .AnyAsync(h => h.CompanyId == companyId && h.HolidayDate.Date == date.Date);

    public async Task<Holiday?> GetByIdAsync(int id, int companyId)
        => await _context.Holidays.FirstOrDefaultAsync(h => h.Id == id && h.CompanyId == companyId);

    public async Task AddAsync(Holiday holiday)
        => await _context.Holidays.AddAsync(holiday);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class EidBonusRepository : IEidBonusRepository
{
    private readonly AppDbContext _context;
    public EidBonusRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<EidBonus>> GetByYearAsync(int companyId, int year)
        => await _context.EidBonuses
            .Include(e => e.Employee).ThenInclude(emp => emp!.Department)
            .Where(e => e.CompanyId == companyId && e.Year == year)
            .OrderBy(e => e.Employee!.EmployeeCode)
            .ToListAsync();

    public async Task<IReadOnlyList<EidBonus>> GetByEmployeeAsync(int companyId, int employeeId)
        => await _context.EidBonuses
            .Where(e => e.CompanyId == companyId && e.EmployeeId == employeeId)
            .OrderByDescending(e => e.Year)
            .ToListAsync();

    public async Task<EidBonus?> GetByIdAsync(int id, int companyId)
        => await _context.EidBonuses
            .Include(e => e.Employee)
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId);

    public async Task AddAsync(EidBonus bonus)
        => await _context.EidBonuses.AddAsync(bonus);

    public async Task AddRangeAsync(IEnumerable<EidBonus> bonuses)
        => await _context.EidBonuses.AddRangeAsync(bonuses);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class JobCardRepository : IJobCardRepository
{
    private readonly AppDbContext _context;
    public JobCardRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<JobCard>> GetByDateAsync(int companyId, DateTime date)
        => await _context.JobCards
            .Include(j => j.Employee).ThenInclude(e => e!.Department)
            .Include(j => j.Line)
            .Where(j => j.CompanyId == companyId && j.WorkDate.Date == date.Date)
            .OrderBy(j => j.Employee!.EmployeeCode)
            .ToListAsync();

    public async Task<IReadOnlyList<JobCard>> GetByDateRangeAsync(int companyId, DateTime from, DateTime to)
        => await _context.JobCards
            .Include(j => j.Employee).ThenInclude(e => e!.Department)
            .Include(j => j.Line)
            .Where(j => j.CompanyId == companyId && j.WorkDate.Date >= from.Date && j.WorkDate.Date <= to.Date)
            .OrderByDescending(j => j.WorkDate).ThenBy(j => j.Employee!.EmployeeCode)
            .ToListAsync();

    public async Task<IReadOnlyList<JobCard>> GetByLineAsync(int companyId, int lineId, DateTime date)
        => await _context.JobCards
            .Include(j => j.Employee)
            .Where(j => j.CompanyId == companyId && j.LineId == lineId && j.WorkDate.Date == date.Date)
            .OrderBy(j => j.Employee!.EmployeeCode)
            .ToListAsync();

    public async Task<IReadOnlyList<JobCard>> GetByEmployeeAsync(int companyId, int employeeId)
        => await _context.JobCards
            .Include(j => j.Line)
            .Where(j => j.CompanyId == companyId && j.EmployeeId == employeeId)
            .OrderByDescending(j => j.WorkDate)
            .ToListAsync();

    public async Task<JobCard?> GetByIdAsync(int id, int companyId)
        => await _context.JobCards
            .Include(j => j.Employee).Include(j => j.Line)
            .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == companyId);

    public async Task AddAsync(JobCard jobCard)
        => await _context.JobCards.AddAsync(jobCard);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
