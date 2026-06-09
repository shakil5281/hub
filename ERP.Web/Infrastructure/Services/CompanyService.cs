using ERP.Web.Areas.Company.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CompanyService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<CompanyListItemVm>> GetAllAsync()
    {
        return await _context.Companies
            .OrderBy(c => c.Name)
            .Select(c => new CompanyListItemVm
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Email = c.Email,
                Phone = c.Phone,
                RegistrationNo = c.RegistrationNo,
                TaxId = c.TaxId,
                Status = c.Status
            })
            .ToListAsync();
    }

    public async Task<CompanyDetailsVm?> GetDetailsAsync(int id)
    {
        return await _context.Companies
            .Where(c => c.Id == id)
            .Select(c => new CompanyDetailsVm
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                RegistrationNo = c.RegistrationNo,
                TaxId = c.TaxId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CompanyEditVm?> GetForEditAsync(int id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        return company == null ? null : MapToEditVm(company);
    }

    public async Task<CompanyEditVm?> GetCompanyAsync()
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == _currentUserService.CompanyId);
        return company == null ? null : MapToEditVm(company);
    }

    public async Task<(bool Success, string? Error)> CreateAsync(CompanyEditVm model)
    {
        var code = model.Code.Trim();
        if (await _context.Companies.AnyAsync(c => c.Code == code))
            return (false, "Company code already exists.");

        _context.Companies.Add(new Core.Entities.Company
        {
            Name = model.Name.Trim(),
            Code = code,
            Address = model.Address?.Trim(),
            Phone = model.Phone?.Trim(),
            Email = model.Email?.Trim(),
            RegistrationNo = model.RegistrationNo?.Trim(),
            TaxId = model.TaxId?.Trim(),
            Status = model.Status,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserName
        });

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(CompanyEditVm model)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == model.Id);
        if (company == null) return (false, "Company not found.");

        var code = model.Code.Trim();
        if (await _context.Companies.AnyAsync(c => c.Code == code && c.Id != model.Id))
            return (false, "Company code already exists.");

        ApplyEdit(company, model);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public Task<(bool Success, string? Error)> UpdateCompanyAsync(CompanyEditVm model)
        => UpdateAsync(model);

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        if (company == null) return (false, "Company not found.");

        var hasEmployees = await _context.Employees.AnyAsync(e => e.CompanyId == id);
        if (hasEmployees) return (false, "Cannot delete company with existing employees.");

        company.IsDeleted = true;
        company.UpdatedAt = DateTime.UtcNow;
        company.UpdatedBy = _currentUserService.UserName;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private static CompanyEditVm MapToEditVm(Core.Entities.Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Code = company.Code,
        Address = company.Address,
        Phone = company.Phone,
        Email = company.Email,
        RegistrationNo = company.RegistrationNo,
        TaxId = company.TaxId,
        Status = company.Status
    };

    private void ApplyEdit(Core.Entities.Company company, CompanyEditVm model)
    {
        company.Name = model.Name.Trim();
        company.Code = model.Code.Trim();
        company.Address = model.Address?.Trim();
        company.Phone = model.Phone?.Trim();
        company.Email = model.Email?.Trim();
        company.RegistrationNo = model.RegistrationNo?.Trim();
        company.TaxId = model.TaxId?.Trim();
        company.Status = model.Status;
        company.UpdatedAt = DateTime.UtcNow;
        company.UpdatedBy = _currentUserService.UserName;
    }
}
