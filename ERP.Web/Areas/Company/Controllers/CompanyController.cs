using ERP.Web.Areas.Company.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Company.Controllers;

[Area("Company")]
public class CompanyController : Controller
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService) => _companyService = companyService;

    [RequirePermission("Company.View")]
    public async Task<IActionResult> Index() => View(await _companyService.GetAllAsync());

    [RequirePermission("Company.View")]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _companyService.GetDetailsAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [RequirePermission("Company.Edit")]
    public IActionResult Create() => View(new CompanyEditVm());

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Company.Edit")]
    public async Task<IActionResult> Create(CompanyEditVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _companyService.CreateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Company created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Company.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _companyService.GetForEditAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Company.Edit")]
    public async Task<IActionResult> Edit(CompanyEditVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _companyService.UpdateAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Company updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Company.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _companyService.DeleteAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Company deleted successfully." : result.Error;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Addresses() => RedirectToAction("Index", "Address", new { area = "HR" });
    public IActionResult CreateAddress() => RedirectToAction("Create", "Address", new { area = "HR" });
    public IActionResult EditAddress(int id) => RedirectToAction("Edit", "Address", new { area = "HR", id });
}
