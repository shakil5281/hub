using Microsoft.AspNetCore.Identity;

namespace ERP.Web.Core.Entities.Security;

public class ApplicationUser : IdentityUser
{
    public int CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
}
