using ERP.Web.Areas.Admin.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RoleController : Controller
{
    private readonly IRoleManagementService _roleService;

    public RoleController(IRoleManagementService roleService) => _roleService = roleService;

    [RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Index() => View(await _roleService.GetAllAsync());

    [RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Permissions = await _roleService.GetAllPermissionsAsync();
        return View(new RoleCreateVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Create(RoleCreateVm model, int[] selectedPermissions)
    {
        model.PermissionIds = selectedPermissions?.ToList() ?? new List<int>();
        if (!ModelState.IsValid) { ViewBag.Permissions = await _roleService.GetAllPermissionsAsync(); return View(model); }
        var result = await _roleService.CreateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Permissions = await _roleService.GetAllPermissionsAsync(); return View(model); }
        TempData["Success"] = "Role created."; return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Edit(string id)
    {
        var model = await _roleService.GetForEditAsync(id);
        if (model == null) return NotFound();
        ViewBag.Permissions = await _roleService.GetAllPermissionsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Edit(RoleEditVm model, int[] selectedPermissions)
    {
        model.PermissionIds = selectedPermissions?.ToList() ?? new List<int>();
        if (!ModelState.IsValid) { ViewBag.Permissions = await _roleService.GetAllPermissionsAsync(); return View(model); }
        var result = await _roleService.UpdateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Permissions = await _roleService.GetAllPermissionsAsync(); return View(model); }
        TempData["Success"] = "Role updated."; return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.Role.Manage")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _roleService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Role deleted." : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
