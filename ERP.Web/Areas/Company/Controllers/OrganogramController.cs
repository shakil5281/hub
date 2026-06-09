using ERP.Web.Areas.Company.ViewModels;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.Company.Controllers;

[Area("Company")]
public class OrganogramController : Controller
{
    private readonly IOrganogramService _organogramService;

    public OrganogramController(IOrganogramService organogramService) => _organogramService = organogramService;

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> Index(string tab = "departments")
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "departments", "sections", "designations", "lines" };
        if (!allowed.Contains(tab)) tab = "departments";
        return View(await _organogramService.GetIndexDataAsync(tab));
    }

    public IActionResult Departments() => RedirectToAction(nameof(Index), new { tab = "departments" });
    public IActionResult Sections() => RedirectToAction(nameof(Index), new { tab = "sections" });
    public IActionResult Lines() => RedirectToAction(nameof(Index), new { tab = "lines" });
    public IActionResult Designations() => RedirectToAction(nameof(Index), new { tab = "designations" });

    [RequirePermission("Organogram.Manage")]
    public IActionResult CreateDepartment() => View(new DepartmentVm());

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateDepartment(DepartmentVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _organogramService.SaveDepartmentAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Department created successfully.";
        return RedirectToAction(nameof(Index), new { tab = "departments" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditDepartment(int id)
    {
        var model = await _organogramService.GetDepartmentAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditDepartment(DepartmentVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _organogramService.SaveDepartmentAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); return View(model); }
        TempData["Success"] = "Department updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "departments" });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var result = await _organogramService.DeleteDepartmentAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Department deleted." : result.Error;
        return RedirectToAction(nameof(Index), new { tab = "departments" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateSection()
    {
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        return View(new SectionVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateSection(SectionVm model)
    {
        if (!ModelState.IsValid) { ViewBag.Departments = await _organogramService.GetDepartmentsAsync(); return View(model); }
        var result = await _organogramService.SaveSectionAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Departments = await _organogramService.GetDepartmentsAsync(); return View(model); }
        TempData["Success"] = "Section created successfully.";
        return RedirectToAction(nameof(Index), new { tab = "sections" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditSection(int id)
    {
        var model = await _organogramService.GetSectionAsync(id);
        if (model == null) return NotFound();
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditSection(SectionVm model)
    {
        if (!ModelState.IsValid) { ViewBag.Departments = await _organogramService.GetDepartmentsAsync(); return View(model); }
        var result = await _organogramService.SaveSectionAsync(model);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error!); ViewBag.Departments = await _organogramService.GetDepartmentsAsync(); return View(model); }
        TempData["Success"] = "Section updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "sections" });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var result = await _organogramService.DeleteSectionAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Section deleted." : result.Error;
        return RedirectToAction(nameof(Index), new { tab = "sections" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateLine()
    {
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync();
        return View(new LineVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateLine(LineVm model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        var result = await _organogramService.SaveLineAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        TempData["Success"] = "Line created successfully.";
        return RedirectToAction(nameof(Index), new { tab = "lines" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditLine(int id)
    {
        var model = await _organogramService.GetLineAsync(id);
        if (model == null) return NotFound();
        var section = (await _organogramService.GetSectionsAsync()).FirstOrDefault(s => s.Id == model.SectionId);
        model.DepartmentId = section?.DepartmentId ?? 0;
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditLine(LineVm model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        var result = await _organogramService.SaveLineAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        TempData["Success"] = "Line updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "lines" });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> DeleteLine(int id)
    {
        var result = await _organogramService.DeleteLineAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Line deleted." : result.Error;
        return RedirectToAction(nameof(Index), new { tab = "lines" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateDesignation()
    {
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync();
        return View(new DesignationVm());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> CreateDesignation(DesignationVm model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        var result = await _organogramService.SaveDesignationAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        TempData["Success"] = "Designation created successfully.";
        return RedirectToAction(nameof(Index), new { tab = "designations" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditDesignation(int id)
    {
        var model = await _organogramService.GetDesignationAsync(id);
        if (model == null) return NotFound();
        ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
        ViewBag.Sections = await _organogramService.GetSectionsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> EditDesignation(DesignationVm model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        var result = await _organogramService.SaveDesignationAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            ViewBag.Departments = await _organogramService.GetDepartmentsAsync();
            ViewBag.Sections = await _organogramService.GetSectionsAsync();
            return View(model);
        }
        TempData["Success"] = "Designation updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "designations" });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> DeleteDesignation(int id)
    {
        var result = await _organogramService.DeleteDesignationAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "Designation deleted." : result.Error;
        return RedirectToAction(nameof(Index), new { tab = "designations" });
    }

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> GetSections(int departmentId) => Json(await _organogramService.GetSectionsAsync(departmentId));

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> GetLines(int sectionId) => Json(await _organogramService.GetLinesAsync(sectionId));

    [RequirePermission("Organogram.Manage")]
    public async Task<IActionResult> GetDesignations(int sectionId) => Json(await _organogramService.GetDesignationsAsync(sectionId));
}
