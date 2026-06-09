using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Areas.Admin.ViewModels;

public class UserListItemVm
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public bool IsActive { get; set; }
}

public class UserCreateVm
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = string.Empty;
    [Required] public string RoleName { get; set; } = "Admin";
}

public class UserEditVm
{
    public string Id { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class RoleListItemVm
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PermissionCount { get; set; }
}

public class RoleCreateVm
{
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IList<int> PermissionIds { get; set; } = new List<int>();
}

public class RoleEditVm : RoleCreateVm
{
    public string Id { get; set; } = string.Empty;
}

public class PermissionItemVm
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
