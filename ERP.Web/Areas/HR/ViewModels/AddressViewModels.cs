using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Areas.HR.ViewModels;

public class AddressVm
{
    public int Id { get; set; }
    [Required] public string Label { get; set; } = string.Empty;
    [Required] public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    [Required] public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public bool IsPrimary { get; set; }

    public string DisplayName => string.IsNullOrEmpty(City) ? Label : $"{Label} — {City}";
}
