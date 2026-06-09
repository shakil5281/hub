using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Interfaces.Services;

// ─── Temporary Shift ────────────────────────────────────────────────────────
public interface ITemporaryShiftService
{
    Task<IReadOnlyList<TemporaryShiftVm>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<(bool Success, string? Error)> SaveAsync(TemporaryShiftCreateVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

// ─── Eid Bonus ───────────────────────────────────────────────────────────────
public interface IEidBonusService
{
    Task<IReadOnlyList<EidBonusVm>> GetByYearAsync(int year, int bonusType);
    Task<(bool Success, string? Error)> SaveAsync(EidBonusVm model);
    Task<(bool Success, string? Error)> GenerateBulkAsync(int year, int bonusType, decimal percentage);
    Task<(bool Success, string? Error)> ApproveAsync(int id);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

// ─── Job Card ─────────────────────────────────────────────────────────────────
public interface IJobCardService
{
    Task<IReadOnlyList<JobCardVm>> GetByDateRangeAsync(DateTime from, DateTime to, int? lineId);
    Task<(bool Success, string? Error)> SaveAsync(JobCardCreateVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

// ─── Monthly Report ───────────────────────────────────────────────────────────
public interface IMonthlyReportService
{
    Task<MonthlyReportVm> GenerateAsync(int year, int month);
}

// ─── ViewModels ───────────────────────────────────────────────────────────────
public record TemporaryShiftVm(int Id, int EmployeeId, string EmployeeName, string EmployeeCode, string Department,
    int ShiftId, string ShiftName, DateTime AssignmentDate, string? Reason);

public class TemporaryShiftCreateVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public DateTime AssignmentDate { get; set; } = DateTime.Today;
    public string? Reason { get; set; }
}

public record EidBonusVm(int Id, int EmployeeId, string EmployeeName, string EmployeeCode, string Department,
    int BonusType, string BonusTypeName, int Year, decimal BonusAmount, int BonusStatus, string BonusStatusName,
    string? Remarks);

public record EidBonusSummaryVm(string BonusTypeName, int Year, int EmployeeCount, decimal TotalAmount, int PaidCount);

public class JobCardCreateVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LineId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public string? TaskDescription { get; set; }
    public decimal TargetQty { get; set; }
    public decimal AchievedQty { get; set; }
    public string? Remarks { get; set; }
}

public record JobCardVm(int Id, int EmployeeId, string EmployeeName, string EmployeeCode, string LineName,
    string Department, DateTime WorkDate, string? TaskDescription, decimal TargetQty, decimal AchievedQty,
    decimal Efficiency, string? Remarks);

public record ManpowerVm(string Department, string Section, string Line, int Total, int Present, int Absent,
    int OnLeave, int Holiday);

public record AbsentStatusVm(string EmployeeCode, string EmployeeName, string Department, string Section,
    DateTime Date, bool IsOnLeave, bool IsHoliday, string LeaveType);

public record DailyOvertimeVm(string EmployeeCode, string EmployeeName, string Department, DateTime Date,
    DateTime? InTime, DateTime? OutTime, int OvertimeMinutes, decimal OvertimeHours, decimal OvertimeAmount);

public record DailyOvertimeSummaryVm(string EmployeeCode, string EmployeeName, string Department,
    int TotalOtMinutes, decimal TotalOtHours, decimal TotalOtAmount, int OtDays);

public record OtDeductionVm(string EmployeeCode, string EmployeeName, string Department,
    int OvertimeMinutes, decimal OvertimeAmount, decimal AbsentDeduction, decimal NetAdjustment);

public record HolidayVm(int Id, string Name, DateTime HolidayDate, string HolidayTypeName, int HolidayType, string? Description);

public class HolidayCreateVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; } = DateTime.Today;
    public int HolidayType { get; set; } = 1;
    public string? Description { get; set; }
}

public record MonthlyLeaveRecordVm(string EmployeeCode, string EmployeeName, string Department,
    int TotalLeaves, int ApprovedLeaves, int PendingLeaves, int RejectedLeaves,
    IReadOnlyList<LeaveDetailRowVm> Details);

public record LeaveDetailRowVm(string LeaveType, DateTime FromDate, DateTime ToDate, int Days, string Status);

public record EarnLeaveVm(string EmployeeCode, string EmployeeName, string Department,
    int AllocatedDays, int UsedDays, int RemainingDays, int Year);

public record DailySalarySheetVm(string EmployeeCode, string EmployeeName, string Department,
    decimal GrossSalary, int WorkingDays, decimal DailyRate, DateTime Date,
    bool IsPresent, bool IsAbsent, bool IsHoliday, decimal PayableAmount);

public record DailySalarySummaryVm(DateTime Date, int PresentCount, int AbsentCount, int HolidayCount,
    decimal TotalPayable, decimal TotalAbsentDeduction);

public record MonthlyReportVm(int Year, int Month, string MonthName,
    int TotalEmployees, int TotalPresent, int TotalAbsent, int TotalLeaves, int TotalHolidays,
    decimal TotalOtHours, decimal TotalPayroll, decimal TotalBonus, decimal TotalAdvance,
    IReadOnlyList<MonthlyReportDeptVm> ByDepartment);

public record MonthlyReportDeptVm(string Department, int EmployeeCount, int PresentDays,
    int AbsentDays, decimal TotalGross, decimal TotalNet, decimal TotalOt);
