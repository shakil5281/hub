using System.Text.Json;
using ERP.Web.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers;

[Authorize]
public class NavbarController : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly INavbarService _navbarService;

    public NavbarController(INavbarService navbarService)
    {
        _navbarService = navbarService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? q)
        => Json(await _navbarService.SearchAsync(q), JsonOptions);

    [HttpGet]
    public async Task<IActionResult> QuickLinks()
        => Json(await _navbarService.GetQuickLinksAsync(), JsonOptions);

    [HttpGet]
    public async Task<IActionResult> Notifications()
        => Json(await _navbarService.GetNotificationsAsync(), JsonOptions);
}
