using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Shift : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GraceMinutes { get; set; }
    public int PunchWindowBeforeMinutes { get; set; }
    public int MaxOvertimeMinutes { get; set; }

    public Company? Company { get; set; }
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
}
