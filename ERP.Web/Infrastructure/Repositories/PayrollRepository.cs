using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly AppDbContext _context;

    public PayrollRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PayrollPeriod>> GetPeriodsAsync(int companyId)
        => await _context.PayrollPeriods
            .Where(p => p.CompanyId == companyId)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();

    public async Task<PayrollPeriod?> GetPeriodByIdAsync(int id, int companyId)
        => await _context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

    public async Task<PayrollSheet?> GetSheetByIdAsync(int id, int companyId)
        => await _context.PayrollSheets
            .Include(s => s.PayrollPeriod)
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);

    public async Task<IReadOnlyList<PayrollDetail>> GetDetailsBySheetAsync(int sheetId, int companyId)
        => await _context.PayrollDetails
            .Include(d => d.Employee)
            .ThenInclude(e => e!.Department)
            .Where(d => d.PayrollSheetId == sheetId && d.CompanyId == companyId)
            .OrderBy(d => d.Employee!.EmployeeCode)
            .ToListAsync();

    public async Task AddPeriodAsync(PayrollPeriod period)
        => await _context.PayrollPeriods.AddAsync(period);

    public async Task AddSheetAsync(PayrollSheet sheet)
        => await _context.PayrollSheets.AddAsync(sheet);

    public async Task AddDetailAsync(PayrollDetail detail)
        => await _context.PayrollDetails.AddAsync(detail);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
