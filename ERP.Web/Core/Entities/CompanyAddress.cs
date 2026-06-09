using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class CompanyAddress : BaseEntity
{
    public string Label { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public bool IsPrimary { get; set; }

    public Company? Company { get; set; }
}
