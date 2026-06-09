using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class SalaryIncrement : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal PreviousGross { get; set; }
    public decimal NewGross { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }

    public Employee? Employee { get; set; }
}
