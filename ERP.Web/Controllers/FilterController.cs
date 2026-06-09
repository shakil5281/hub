using ERP.Web.Core.DTOs;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers;

public class FilterController : Controller
{
    private readonly IFilterPresetService _filterPresetService;

    public FilterController(IFilterPresetService filterPresetService) => _filterPresetService = filterPresetService;

    [HttpGet]
    public async Task<IActionResult> Presets(string module) => Json(await _filterPresetService.GetPresetsAsync(module));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePreset(string module, string name, string filterJson, bool isDefault)
    {
        var result = await _filterPresetService.SavePresetAsync(module, name, filterJson, isDefault);
        return Json(new { success = result.Success, error = result.Error });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePreset(int id)
    {
        var result = await _filterPresetService.DeletePresetAsync(id);
        return Json(new { success = result.Success, error = result.Error });
    }
}
