using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class EmployeeAddress : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? AddressType { get; set; }
    public string? Division { get; set; }
    public string? District { get; set; }
    public string? Upazila { get; set; }
    public string? PostOffice { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }

    public Employee? Employee { get; set; }
}

public class EmployeeJobHistory : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Reason { get; set; }

    public Employee? Employee { get; set; }
    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public Line? Line { get; set; }
    public Designation? Designation { get; set; }
}

public class EmployeeSalaryStructure : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal FoodAllowance { get; set; }
    public decimal ConveyanceAllowance { get; set; }
    public decimal OtherAllowance { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
    public ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
}

public class EmployeeDeviceMapping : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? PunchCardNo { get; set; }
    public string? DeviceUserId { get; set; }
    public string? DeviceId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
    public ICollection<PunchRecord> PunchRecords { get; set; } = new List<PunchRecord>();
}

public class EmployeeWeeklyOff : BaseEntity
{
    public int EmployeeId { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime? OffDate { get; set; }
    public string? OffType { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
}

public class DeviceMaster : BaseEntity
{
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string? DeviceType { get; set; }
    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public string? LocationName { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Company? Company { get; set; }
    public ICollection<DeviceSyncLog> SyncLogs { get; set; } = new List<DeviceSyncLog>();
    public ICollection<PunchRecord> PunchRecords { get; set; } = new List<PunchRecord>();
}

public class DeviceSyncLog : BaseEntity
{
    public int DeviceMasterId { get; set; }
    public DateTime SyncStartedAt { get; set; }
    public DateTime? SyncEndedAt { get; set; }
    public int TotalRecords { get; set; }
    public int NewRecords { get; set; }
    public int DuplicateRecords { get; set; }
    public int FailedRecords { get; set; }
    public string? SyncStatus { get; set; }
    public string? ErrorMessage { get; set; }

    public DeviceMaster? DeviceMaster { get; set; }
}

public class ManualPunchRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime RequestedPunchTime { get; set; }
    public string? PunchType { get; set; }
    public string? Reason { get; set; }
    public string? RequestStatus { get; set; }
    public string? RequestedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CreatedPunchRecordId { get; set; }

    public Employee? Employee { get; set; }
    public PunchRecord? CreatedPunchRecord { get; set; }
}

public class DailyAttendancePunch : BaseEntity
{
    public int DailyAttendanceId { get; set; }
    public int PunchRecordId { get; set; }
    public string? PunchUsageType { get; set; }

    public DailyAttendance? DailyAttendance { get; set; }
    public PunchRecord? PunchRecord { get; set; }
}

public class AttendanceApprovalLog : BaseEntity
{
    public int DailyAttendanceId { get; set; }
    public string? Action { get; set; }
    public string? ActionBy { get; set; }
    public DateTime ActionAt { get; set; }
    public string? Remarks { get; set; }

    public DailyAttendance? DailyAttendance { get; set; }
}

public class OvertimeApprovalLog : BaseEntity
{
    public int DailyAttendanceId { get; set; }
    public int EmployeeId { get; set; }
    public int RawOvertimeMinutes { get; set; }
    public int RuleOvertimeMinutes { get; set; }
    public int ApprovedOvertimeMinutes { get; set; }
    public string? Action { get; set; }
    public string? ActionBy { get; set; }
    public DateTime ActionAt { get; set; }
    public string? Remarks { get; set; }

    public DailyAttendance? DailyAttendance { get; set; }
    public Employee? Employee { get; set; }
}

public class AttendanceConflict : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string? ConflictType { get; set; }
    public string? Description { get; set; }
    public string? ConflictStatus { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Employee? Employee { get; set; }
}

public class MonthlyAttendanceSummary : BaseEntity
{
    public int EmployeeId { get; set; }
    public int SalaryYear { get; set; }
    public int SalaryMonth { get; set; }
    public int CalendarDays { get; set; }
    public decimal PayableDays { get; set; }
    public decimal PresentDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal PaidLeaveDays { get; set; }
    public decimal UnpaidLeaveDays { get; set; }
    public decimal HolidayDays { get; set; }
    public decimal HolidayPresentDays { get; set; }
    public decimal WeeklyOffDays { get; set; }
    public decimal WeeklyOffPresentDays { get; set; }
    public decimal LateDays { get; set; }
    public decimal EarlyOutDays { get; set; }
    public decimal MissingPunchDays { get; set; }
    public int TotalWorkMinutes { get; set; }
    public int TotalPayableOtMinutes { get; set; }
    public int NightDutyDays { get; set; }
    public bool IsApproved { get; set; }
    public bool IsPayrollLocked { get; set; }

    public Employee? Employee { get; set; }
    public ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
}

public class LeavePolicy : BaseEntity
{
    public int LeaveTypeId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public decimal YearlyQuota { get; set; }
    public decimal MonthlyAccrual { get; set; }
    public decimal MaxCarryForwardDays { get; set; }
    public decimal MaxEncashmentDays { get; set; }
    public bool ExcludeHolidayFromLeave { get; set; }
    public bool ExcludeWeeklyOffFromLeave { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public LeaveType? LeaveType { get; set; }
}

public class LeaveApprovalLog : BaseEntity
{
    public int LeaveApplicationId { get; set; }
    public int ApprovalLevel { get; set; }
    public string? Action { get; set; }
    public string? ActionBy { get; set; }
    public DateTime ActionAt { get; set; }
    public string? Remarks { get; set; }

    public LeaveApplication? LeaveApplication { get; set; }
}

public class LeaveBalanceTransaction : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int? LeaveApplicationId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? TransactionType { get; set; }
    public decimal Days { get; set; }
    public string? Remarks { get; set; }

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
    public LeaveApplication? LeaveApplication { get; set; }
}

public class LeaveConflict : BaseEntity
{
    public int LeaveApplicationId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ConflictDate { get; set; }
    public string? ConflictType { get; set; }
    public string? ConflictStatus { get; set; }
    public string? ResolutionAction { get; set; }

    public LeaveApplication? LeaveApplication { get; set; }
    public Employee? Employee { get; set; }
}

public class NightBillRule : BaseEntity
{
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    public string? CalculationType { get; set; }
    public decimal FixedAmount { get; set; }
    public decimal HourlyRate { get; set; }
    public TimeSpan? NightStartTime { get; set; }
    public TimeSpan? NightEndTime { get; set; }
    public int MinimumNightWorkMinutes { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Department? Department { get; set; }
    public Designation? Designation { get; set; }
}

public class SpecialDayBillRule : BaseEntity
{
    public string? BillType { get; set; }
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    public string? CalculationType { get; set; }
    public decimal FixedAmount { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Multiplier { get; set; }
    public int MinimumWorkMinutes { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Department? Department { get; set; }
    public Designation? Designation { get; set; }
}

public class AttendanceBonusRule : BaseEntity
{
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public decimal BonusAmount { get; set; }
    public decimal MaxAbsentAllowed { get; set; }
    public decimal MaxUnpaidLeaveAllowed { get; set; }
    public int MaxLateAllowed { get; set; }
    public int MaxEarlyOutAllowed { get; set; }
    public int MaxMissingPunchAllowed { get; set; }
    public decimal MinimumPresentDays { get; set; }
    public decimal MinimumPayableDays { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Department? Department { get; set; }
    public Designation? Designation { get; set; }
}

public class EmployeeBillEntry : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? DailyAttendanceId { get; set; }
    public string? BillType { get; set; }
    public int BillYear { get; set; }
    public int BillMonth { get; set; }
    public string? SourceType { get; set; }
    public int? SourceReferenceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? EntryStatus { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsPayrollLocked { get; set; }

    public Employee? Employee { get; set; }
    public DailyAttendance? DailyAttendance { get; set; }
}

public class EmployeeBonusEntry : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? MonthlyAttendanceSummaryId { get; set; }
    public int? AttendanceBonusRuleId { get; set; }
    public string? BonusType { get; set; }
    public int BonusYear { get; set; }
    public int BonusMonth { get; set; }
    public string? SourceType { get; set; }
    public int? SourceReferenceId { get; set; }
    public decimal Amount { get; set; }
    public string? EligibilityStatus { get; set; }
    public string? NotEligibleReason { get; set; }
    public string? EntryStatus { get; set; }
    public bool IsPayrollLocked { get; set; }

    public Employee? Employee { get; set; }
    public MonthlyAttendanceSummary? MonthlyAttendanceSummary { get; set; }
    public AttendanceBonusRule? AttendanceBonusRule { get; set; }
}

public class Loan : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? LoanStatus { get; set; }

    public Employee? Employee { get; set; }
    public ICollection<LoanInstallment> Installments { get; set; } = new List<LoanInstallment>();
}

public class LoanInstallment : BaseEntity
{
    public int LoanId { get; set; }
    public int InstallmentYear { get; set; }
    public int InstallmentMonth { get; set; }
    public decimal InstallmentAmount { get; set; }
    public string? InstallmentStatus { get; set; }

    public Loan? Loan { get; set; }
}

public class PayrollRun : BaseEntity
{
    public int SalaryYear { get; set; }
    public int SalaryMonth { get; set; }
    public string? PayrollType { get; set; }
    public string? RunStatus { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
    public bool IsLocked { get; set; }

    public Company? Company { get; set; }
    public ICollection<PayrollRunDetail> Details { get; set; } = new List<PayrollRunDetail>();
}

public class PayrollRunDetail : BaseEntity
{
    public int PayrollRunId { get; set; }
    public int EmployeeId { get; set; }
    public int? SalaryStructureId { get; set; }
    public int? MonthlyAttendanceSummaryId { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal NightBillAmount { get; set; }
    public decimal HolidayBillAmount { get; set; }
    public decimal WeeklyOffBillAmount { get; set; }
    public decimal AttendanceBonusAmount { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal NetPayable { get; set; }
    public string? DetailStatus { get; set; }

    public PayrollRun? PayrollRun { get; set; }
    public Employee? Employee { get; set; }
    public EmployeeSalaryStructure? SalaryStructure { get; set; }
    public MonthlyAttendanceSummary? MonthlyAttendanceSummary { get; set; }
    public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
}

public class Payslip : BaseEntity
{
    public int PayrollRunDetailId { get; set; }
    public int EmployeeId { get; set; }
    public int SalaryYear { get; set; }
    public int SalaryMonth { get; set; }
    public decimal NetPayable { get; set; }
    public string? PayslipNo { get; set; }
    public DateTime GeneratedAt { get; set; }

    public PayrollRunDetail? PayrollRunDetail { get; set; }
    public Employee? Employee { get; set; }
}

public class AuditLog : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public int? RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ActionBy { get; set; }
    public DateTime ActionAt { get; set; }
    public string? IpAddress { get; set; }
}

public class ServiceErrorLog : BaseEntity
{
    public string ServiceName { get; set; } = string.Empty;
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RequestPayload { get; set; }
}
