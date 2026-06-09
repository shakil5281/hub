using Microsoft.AspNetCore.Identity;

namespace ERP.Web.Core.Entities.Security;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}
