using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class LeaveType : BaseEntity
{
    public string? LeaveCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxDaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHalfDayAllowed { get; set; }
    public bool IsCarryForwardAllowed { get; set; }
    public bool IsEncashmentAllowed { get; set; }

    public Company? Company { get; set; }
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}
