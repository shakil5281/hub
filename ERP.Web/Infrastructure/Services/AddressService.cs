using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddressService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<AddressVm>> GetAllAsync()
    {
        var companyId = _currentUserService.CompanyId;
        return await _context.CompanyAddresses
            .Where(a => a.CompanyId == companyId)
            .OrderByDescending(a => a.IsPrimary)
            .ThenBy(a => a.Label)
            .Select(a => MapToVm(a))
            .ToListAsync();
    }

    public async Task<AddressVm?> GetAsync(int id)
    {
        var companyId = _currentUserService.CompanyId;
        var address = await _context.CompanyAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == companyId);
        return address == null ? null : MapToVm(address);
    }

    public async Task<(bool Success, string? Error)> SaveAsync(AddressVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.IsPrimary)
        {
            var existingPrimary = await _context.CompanyAddresses
                .Where(a => a.CompanyId == companyId && a.IsPrimary && a.Id != model.Id)
                .ToListAsync();
            foreach (var addr in existingPrimary)
                addr.IsPrimary = false;
        }

        if (model.Id == 0)
        {
            _context.CompanyAddresses.Add(new Core.Entities.CompanyAddress
            {
                CompanyId = companyId,
                Label = model.Label.Trim(),
                AddressLine1 = model.AddressLine1.Trim(),
                AddressLine2 = model.AddressLine2?.Trim(),
                City = model.City.Trim(),
                State = model.State?.Trim(),
                PostalCode = model.PostalCode?.Trim(),
                IsPrimary = model.IsPrimary,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var address = await _context.CompanyAddresses
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.CompanyId == companyId);
            if (address == null) return (false, "Address not found.");

            address.Label = model.Label.Trim();
            address.AddressLine1 = model.AddressLine1.Trim();
            address.AddressLine2 = model.AddressLine2?.Trim();
            address.City = model.City.Trim();
            address.State = model.State?.Trim();
            address.PostalCode = model.PostalCode?.Trim();
            address.IsPrimary = model.IsPrimary;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = _currentUserService.UserName;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var companyId = _currentUserService.CompanyId;
        var address = await _context.CompanyAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == companyId);
        if (address == null) return (false, "Address not found.");

        var inUse = await _context.Employees.AnyAsync(e => e.AddressId == id && e.CompanyId == companyId);
        if (inUse) return (false, "Cannot delete address assigned to employees.");

        address.IsDeleted = true;
        address.UpdatedAt = DateTime.UtcNow;
        address.UpdatedBy = _currentUserService.UserName;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private static AddressVm MapToVm(Core.Entities.CompanyAddress a) => new()
    {
        Id = a.Id,
        Label = a.Label,
        AddressLine1 = a.AddressLine1,
        AddressLine2 = a.AddressLine2,
        City = a.City,
        State = a.State,
        PostalCode = a.PostalCode,
        IsPrimary = a.IsPrimary
    };
}
