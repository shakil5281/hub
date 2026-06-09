using System.Security.Claims;
using ERP.Web.Core.Interfaces.Services;

namespace ERP.Web.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => User?.Identity?.Name;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public int CompanyId
    {
        get
        {
            var claim = User?.FindFirstValue("CompanyId");
            return int.TryParse(claim, out var companyId) ? companyId : 0;
        }
    }
}
