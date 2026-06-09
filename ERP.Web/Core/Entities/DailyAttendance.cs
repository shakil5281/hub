using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class DailyAttendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? InTime { get; set; }
    public DateTime? OutTime { get; set; }
    public int LateMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public int WorkedMinutes { get; set; }
    public bool IsAbsent { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsNightShift { get; set; }
    public bool IsApproved { get; set; }
    public bool IsPayrollLocked { get; set; }

    public Employee? Employee { get; set; }
}
