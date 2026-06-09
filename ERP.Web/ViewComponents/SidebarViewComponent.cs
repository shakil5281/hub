using ERP.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var path = ViewContext.HttpContext.Request.Path.Value;
        var model = SidebarNavigation.Build(Url, path);
        return View(model);
    }
}
