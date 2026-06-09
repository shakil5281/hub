using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class JobCard : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public int LineId { get; set; }
    public string? TaskDescription { get; set; }
    public decimal TargetQty { get; set; }
    public decimal AchievedQty { get; set; }
    public string? Remarks { get; set; }

    public Employee? Employee { get; set; }
    public Line? Line { get; set; }
}
