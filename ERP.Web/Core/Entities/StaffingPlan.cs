using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class StaffingPlan : BaseEntity
{
    public int DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int RequiredCount { get; set; }
    public string? Remarks { get; set; }

    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public Line? Line { get; set; }
    public Designation? Designation { get; set; }
}
