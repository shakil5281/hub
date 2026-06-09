using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class LeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public int AllocatedDays { get; set; }
    public int UsedDays { get; set; }
    public int RemainingDays { get; set; }

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
}
