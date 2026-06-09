using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Areas.HR.Controllers;

[Area("HR")]
public class LookupController : Controller
{
    private readonly IOrganogramService _organogramService;

    public LookupController(IOrganogramService organogramService) => _organogramService = organogramService;

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Sections(int departmentId) => Json(await _organogramService.GetSectionsAsync(departmentId));

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Lines(int sectionId) => Json(await _organogramService.GetLinesAsync(sectionId));

    [RequirePermission("HR.Employee.View")]
    public async Task<IActionResult> Designations(int sectionId) => Json(await _organogramService.GetDesignationsAsync(sectionId));
}
