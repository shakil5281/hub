using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class DailyAttendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? ShiftId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? ShiftStartDateTime { get; set; }
    public DateTime? ShiftEndDateTime { get; set; }
    public DateTime? WindowStartDateTime { get; set; }
    public DateTime? WindowEndDateTime { get; set; }
    public DateTime? InTime { get; set; }
    public DateTime? OutTime { get; set; }
    public int LateMinutes { get; set; }
    public int EarlyOutMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public int WorkedMinutes { get; set; }
    public int BreakMinutes { get; set; }
    public int RawOvertimeMinutes { get; set; }
    public int RuleOvertimeMinutes { get; set; }
    public int ApprovedOvertimeMinutes { get; set; }
    public int PayableOvertimeMinutes { get; set; }
    public int NightWorkMinutes { get; set; }
    public string? AttendanceStatus { get; set; }
    public decimal PayableDayValue { get; set; }
    public bool IsPresent { get; set; }
    public bool IsAbsent { get; set; }
    public bool IsLate { get; set; }
    public bool IsEarlyOut { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsHolidayPresent { get; set; }
    public bool IsWeeklyOff { get; set; }
    public bool IsWeeklyOffPresent { get; set; }
    public bool IsLeave { get; set; }
    public bool IsPaidLeave { get; set; }
    public bool IsNightShift { get; set; }
    public bool IsNightDuty { get; set; }
    public bool IsApproved { get; set; }
    public bool IsPayrollLocked { get; set; }

    public Employee? Employee { get; set; }
    public Shift? Shift { get; set; }
}
