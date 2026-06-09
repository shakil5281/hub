namespace ERP.Web.Core.Entities.Security;

public class RolePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public int PermissionId { get; set; }

    public ApplicationRole? Role { get; set; }
    public Permission? Permission { get; set; }
}
