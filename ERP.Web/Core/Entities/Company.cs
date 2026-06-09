using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoPath { get; set; }
    public string? RegistrationNo { get; set; }
    public string? TaxId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    public ICollection<CompanyAddress> Addresses { get; set; } = new List<CompanyAddress>();
}
