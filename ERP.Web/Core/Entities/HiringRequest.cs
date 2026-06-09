using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class HiringRequest : BaseEntity
{
    public int DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public int RequestedCount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? TargetJoinDate { get; set; }
    public string? Reason { get; set; }
    public HiringRequestStatus HiringRequestStatus { get; set; } = HiringRequestStatus.Draft;
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }

    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public Line? Line { get; set; }
    public Designation? Designation { get; set; }
}
