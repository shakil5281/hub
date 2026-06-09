using Microsoft.AspNetCore.Authorization;

namespace ERP.Web.Infrastructure.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionCode)
    {
        Policy = $"Permission:{permissionCode}";
    }
}
