using System.ComponentModel.DataAnnotations;
using ERP.Web.Core.Enums;

namespace ERP.Web.Areas.Company.ViewModels;

public class CompanyListItemVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? RegistrationNo { get; set; }
    public string? TaxId { get; set; }
    public EntityStatus Status { get; set; }
}

public class CompanyEditVm
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    [EmailAddress] public string? Email { get; set; }
    public string? RegistrationNo { get; set; }
    public string? TaxId { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;
}

public class CompanyDetailsVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? RegistrationNo { get; set; }
    public string? TaxId { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class DepartmentVm
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class SectionVm
{
    public int Id { get; set; }
    [Required] public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class LineVm
{
    public int Id { get; set; }
    [Required] public int SectionId { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? SectionName { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class DesignationVm
{
    public int Id { get; set; }
    [Required] public int SectionId { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? SectionName { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
}

public class OrganogramIndexVm
{
    public string ActiveTab { get; set; } = "departments";
    public IReadOnlyList<DepartmentVm> Departments { get; set; } = Array.Empty<DepartmentVm>();
    public IReadOnlyList<SectionVm> Sections { get; set; } = Array.Empty<SectionVm>();
    public IReadOnlyList<LineVm> Lines { get; set; } = Array.Empty<LineVm>();
    public IReadOnlyList<DesignationVm> Designations { get; set; } = Array.Empty<DesignationVm>();
}

public class OrganogramTreeVm
{
    public IList<OrganogramDepartmentNode> Departments { get; set; } = new List<OrganogramDepartmentNode>();
}

public class OrganogramDepartmentNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<OrganogramSectionNode> Sections { get; set; } = new List<OrganogramSectionNode>();
}

public class OrganogramSectionNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<OrganogramDesignationNode> Designations { get; set; } = new List<OrganogramDesignationNode>();
    public IList<OrganogramLineNode> Lines { get; set; } = new List<OrganogramLineNode>();
}

public class OrganogramDesignationNode
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class OrganogramLineNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
