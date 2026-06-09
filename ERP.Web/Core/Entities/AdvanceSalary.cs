using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class AdvanceSalary : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingBalance { get; set; }
    public AdvanceSalaryStatus AdvanceStatus { get; set; } = AdvanceSalaryStatus.Pending;
    public string? Reason { get; set; }

    public Employee? Employee { get; set; }
}
