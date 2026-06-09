using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int MaxDaysPerYear { get; set; }
    public bool IsPaid { get; set; }

    public Company? Company { get; set; }
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}
