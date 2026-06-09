using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class TemporaryShiftAssignment : BaseEntity
{
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public string? Reason { get; set; }

    public Employee? Employee { get; set; }
    public Shift? Shift { get; set; }
}
