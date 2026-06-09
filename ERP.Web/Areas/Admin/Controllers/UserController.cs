using ERP.Web.Areas.Admin.ViewModels;
using ERP.Web.Core.DTOs;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    private readonly IUserManagementService _userService;
    private readonly IRoleManagementService _roleService;

    public UserController(IUserManagementService userService, IRoleManagementService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [RequirePermission("Admin.User.View")]
    public async Task<IActionResult> Index([FromQuery] UserFilterDto filter)
    {
        ViewBag.Filter = filter;
        ViewBag.FilterAction = Url.Action(nameof(Index));
        ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList();
        return View(await _userService.SearchAsync(filter));
    }

    [RequirePermission("Admin.User.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList();
        return View(new UserCreateVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.User.Create")]
    public async Task<IActionResult> Create(UserCreateVm model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList();
            return View(model);
        }
        var result = await _userService.CreateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList(); return View(model); }
        TempData["Success"] = "User created."; return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Admin.User.Edit")]
    public async Task<IActionResult> Edit(string id)
    {
        var model = await _userService.GetForEditAsync(id);
        if (model == null) return NotFound();
        ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.User.Edit")]
    public async Task<IActionResult> Edit(UserEditVm model)
    {
        if (!ModelState.IsValid) { ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList(); return View(model); }
        var result = await _userService.UpdateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Roles = (await _roleService.GetAllAsync()).Select(r => r.Name).ToList(); return View(model); }
        TempData["Success"] = "User updated."; return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Admin.User.Delete")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "User deleted." : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
