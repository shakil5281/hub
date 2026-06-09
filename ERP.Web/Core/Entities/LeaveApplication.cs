using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class LeaveApplication : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDays { get; set; }
    public string? LeaveDurationType { get; set; }
    public LeaveApprovalStatus ApprovalStatus { get; set; } = LeaveApprovalStatus.Pending;
    public string? Reason { get; set; }
    public int CurrentApprovalLevel { get; set; }
    public string? AppliedBy { get; set; }
    public DateTime? AppliedAt { get; set; }
    public string? FinalApprovedBy { get; set; }
    public DateTime? FinalApprovedAt { get; set; }

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
}
