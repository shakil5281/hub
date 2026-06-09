using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Services;

public interface IFilterPresetService
{
    Task<IReadOnlyList<SavedFilter>> GetPresetsAsync(string module);
    Task<SavedFilter?> GetDefaultPresetAsync(string module);
    Task<(bool Success, string? Error)> SavePresetAsync(string module, string name, string filterJson, bool isDefault);
    Task<(bool Success, string? Error)> DeletePresetAsync(int id);
}
