using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Shift : BaseEntity
{
    public string? ShiftCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GraceMinutes { get; set; }
    public int PunchWindowBeforeMinutes { get; set; }
    public int PunchAfterMinutes { get; set; }
    public int BreakMinutes { get; set; }
    public int MinimumWorkMinutes { get; set; }
    public int MinimumOtMinutes { get; set; }
    public int MaxOvertimeMinutes { get; set; }
    public bool IsNightShift { get; set; }

    public Company? Company { get; set; }
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
}
