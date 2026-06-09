using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.HR.Controllers;

[Area("HR")]
public class AddressController : Controller
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService) => _addressService = addressService;

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Index() => View(await _addressService.GetAllAsync());

    [RequirePermission("HR.Employee.Edit")]
    public IActionResult Create() => View(new AddressVm());

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Create(AddressVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _addressService.SaveAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Address created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _addressService.GetAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Edit(AddressVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _addressService.SaveAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Address updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("HR.Employee.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _addressService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Address deleted." : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
