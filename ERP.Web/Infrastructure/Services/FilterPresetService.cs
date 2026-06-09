using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class FilterPresetService : IFilterPresetService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public FilterPresetService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SavedFilter>> GetPresetsAsync(string module)
    {
        var userId = _currentUser.UserId ?? string.Empty;
        return await _context.SavedFilters
            .Where(f => f.CompanyId == _currentUser.CompanyId && f.UserId == userId && f.Module == module)
            .OrderByDescending(f => f.IsDefault)
            .ThenBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<SavedFilter?> GetDefaultPresetAsync(string module)
    {
        var userId = _currentUser.UserId ?? string.Empty;
        return await _context.SavedFilters
            .FirstOrDefaultAsync(f => f.CompanyId == _currentUser.CompanyId && f.UserId == userId && f.Module == module && f.IsDefault);
    }

    public async Task<(bool Success, string? Error)> SavePresetAsync(string module, string name, string filterJson, bool isDefault)
    {
        var userId = _currentUser.UserId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name)) return (false, "Preset name is required.");

        if (isDefault)
        {
            var existing = await _context.SavedFilters
                .Where(f => f.CompanyId == _currentUser.CompanyId && f.UserId == userId && f.Module == module && f.IsDefault)
                .ToListAsync();
            foreach (var item in existing) item.IsDefault = false;
        }

        _context.SavedFilters.Add(new SavedFilter
        {
            CompanyId = _currentUser.CompanyId,
            UserId = userId,
            Module = module,
            Name = name.Trim(),
            FilterJson = filterJson,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserName
        });
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeletePresetAsync(int id)
    {
        var preset = await _context.SavedFilters
            .FirstOrDefaultAsync(f => f.Id == id && f.CompanyId == _currentUser.CompanyId && f.UserId == (_currentUser.UserId ?? string.Empty));
        if (preset == null) return (false, "Preset not found.");
        preset.IsDeleted = true;
        preset.UpdatedAt = DateTime.UtcNow;
        preset.UpdatedBy = _currentUser.UserName;
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
