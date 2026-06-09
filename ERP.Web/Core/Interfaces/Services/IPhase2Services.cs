using ERP.Web.Areas.Admin.ViewModels;
using ERP.Web.Areas.Company.ViewModels;
using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.DTOs;

namespace ERP.Web.Core.Interfaces.Services;

public interface IUserManagementService
{
    Task<IReadOnlyList<UserListItemVm>> GetAllAsync();
    Task<IReadOnlyList<UserListItemVm>> SearchAsync(UserFilterDto filter);
    Task<UserEditVm?> GetForEditAsync(string id);
    Task<(bool Success, string? Error)> CreateAsync(UserCreateVm model);
    Task<(bool Success, string? Error)> UpdateAsync(UserEditVm model);
    Task<(bool Success, string? Error)> DeleteAsync(string id);
    Task<(bool Success, string? Error)> ResetPasswordAsync(string id, string newPassword);
}

public interface IRoleManagementService
{
    Task<IReadOnlyList<RoleListItemVm>> GetAllAsync();
    Task<RoleEditVm?> GetForEditAsync(string id);
    Task<(bool Success, string? Error)> CreateAsync(RoleCreateVm model);
    Task<(bool Success, string? Error)> UpdateAsync(RoleEditVm model);
    Task<(bool Success, string? Error)> DeleteAsync(string id);
    Task<IReadOnlyList<PermissionItemVm>> GetAllPermissionsAsync();
}

public interface ICompanyService
{
    Task<IReadOnlyList<CompanyListItemVm>> GetAllAsync();
    Task<CompanyDetailsVm?> GetDetailsAsync(int id);
    Task<CompanyEditVm?> GetForEditAsync(int id);
    Task<CompanyEditVm?> GetCompanyAsync();
    Task<(bool Success, string? Error)> CreateAsync(CompanyEditVm model);
    Task<(bool Success, string? Error)> UpdateAsync(CompanyEditVm model);
    Task<(bool Success, string? Error)> UpdateCompanyAsync(CompanyEditVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface IAddressService
{
    Task<IReadOnlyList<AddressVm>> GetAllAsync();
    Task<AddressVm?> GetAsync(int id);
    Task<(bool Success, string? Error)> SaveAsync(AddressVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface IOrganogramService
{
    Task<OrganogramIndexVm> GetIndexDataAsync(string activeTab = "departments");
    Task<IReadOnlyList<DepartmentVm>> GetDepartmentsAsync();
    Task<DepartmentVm?> GetDepartmentAsync(int id);
    Task<(bool Success, string? Error)> SaveDepartmentAsync(DepartmentVm model);
    Task<(bool Success, string? Error)> DeleteDepartmentAsync(int id);
    Task<IReadOnlyList<SectionVm>> GetSectionsAsync(int? departmentId = null);
    Task<SectionVm?> GetSectionAsync(int id);
    Task<(bool Success, string? Error)> SaveSectionAsync(SectionVm model);
    Task<(bool Success, string? Error)> DeleteSectionAsync(int id);
    Task<IReadOnlyList<LineVm>> GetLinesAsync(int? sectionId = null);
    Task<LineVm?> GetLineAsync(int id);
    Task<(bool Success, string? Error)> SaveLineAsync(LineVm model);
    Task<(bool Success, string? Error)> DeleteLineAsync(int id);
    Task<IReadOnlyList<DesignationVm>> GetDesignationsAsync(int? sectionId = null);
    Task<DesignationVm?> GetDesignationAsync(int id);
    Task<(bool Success, string? Error)> SaveDesignationAsync(DesignationVm model);
    Task<(bool Success, string? Error)> DeleteDesignationAsync(int id);
    Task<OrganogramTreeVm> GetOrganogramTreeAsync();
}

public interface IShiftService
{
    Task<IReadOnlyList<ShiftListItemVm>> GetAllAsync();
    Task<ShiftEditVm?> GetForEditAsync(int id);
    Task<(bool Success, string? Error)> SaveAsync(ShiftEditVm model);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
    Task<IReadOnlyList<ShiftAssignmentVm>> GetAssignmentsAsync();
    Task<(bool Success, string? Error)> SaveAssignmentAsync(ShiftAssignmentVm model);
}

public interface IPunchRecordService
{
    Task<IReadOnlyList<PunchRecordVm>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<(bool Success, string? Error)> CreateManualAsync(PunchRecordCreateVm model);
}

public interface IPunchImportService
{
    Task<(int Imported, IList<string> Errors)> ImportAsync(Stream stream);
}

public interface IAttendanceProcessService
{
    Task<(int Processed, int Skipped, string? Error)> ProcessAsync(DateTime from, DateTime to);
    Task<IReadOnlyList<DailyAttendanceVm>> GetDailyAttendanceAsync(AttendanceFilterDto filter);
    Task<(bool Success, string? Error)> ApproveAsync(int id);
    Task<IReadOnlyList<AttendanceReportVm>> GetReportAsync(AttendanceFilterDto filter);
    Task<IReadOnlyList<AttendanceSummaryVm>> GetSummaryAsync(AttendanceFilterDto filter);
    Task<IReadOnlyList<ManpowerVm>> GetManpowerReportAsync(DateTime date);
    Task<IReadOnlyList<AbsentStatusVm>> GetAbsentStatusAsync(DateTime date);
    Task<IReadOnlyList<DailyOvertimeVm>> GetDailyOvertimeAsync(DateTime date);
    Task<IReadOnlyList<DailyOvertimeSummaryVm>> GetDailyOvertimeSummaryAsync(DateTime from, DateTime to);
    Task<IReadOnlyList<OtDeductionVm>> GetOtDeductionAsync(int periodId);
}

public interface ILeaveService
{
    Task<IReadOnlyList<LeaveTypeVm>> GetLeaveTypesAsync();
    Task<(bool Success, string? Error)> SaveLeaveTypeAsync(LeaveTypeVm model);
    Task<IReadOnlyList<LeaveBalanceVm>> GetBalancesAsync(int? employeeId);
    Task<(bool Success, string? Error)> AdjustBalanceAsync(LeaveBalanceVm model);
    Task<IReadOnlyList<LeaveApplicationVm>> GetApplicationsAsync();
    Task<(bool Success, string? Error)> SubmitApplicationAsync(LeaveApplicationCreateVm model);
    Task<(bool Success, string? Error)> ApproveAsync(int id, bool approved);
    Task<IReadOnlyList<HolidayVm>> GetHolidaysAsync(int year);
    Task<(bool Success, string? Error)> SaveHolidayAsync(HolidayCreateVm model);
    Task<(bool Success, string? Error)> DeleteHolidayAsync(int id);
    Task<IReadOnlyList<MonthlyLeaveRecordVm>> GetMonthlyLeaveRecordAsync(int year, int month);
    Task<IReadOnlyList<EarnLeaveVm>> GetEarnLeaveReportAsync(int year);
}

public interface IPayrollService
{
    Task<IReadOnlyList<PayrollPeriodVm>> GetPeriodsAsync();
    Task<(bool Success, string? Error, int? SheetId)> GeneratePayrollAsync(int periodId);
    Task<IReadOnlyList<PayrollDetailVm>> GetSheetDetailsAsync(int sheetId);
    Task<IReadOnlyList<PayrollSummaryVm>> GetSummaryAsync(int sheetId);
    Task<PayrollDetailVm?> GetPayslipAsync(int detailId);
    Task<IReadOnlyList<AdvanceSalaryVm>> GetAdvancesAsync();
    Task<(bool Success, string? Error)> SaveAdvanceAsync(AdvanceSalaryVm model);
    Task<(bool Success, string? Error)> ApproveAdvanceAsync(int id);
    Task<IReadOnlyList<SalaryIncrementVm>> GetIncrementsAsync();
    Task<(bool Success, string? Error)> SaveIncrementAsync(SalaryIncrementVm model);
    Task<IReadOnlyList<BillRateConfigVm>> GetBillRatesAsync();
    Task<(bool Success, string? Error)> SaveBillRateAsync(BillRateConfigVm model);
    Task<IReadOnlyList<DailySalarySheetVm>> GetDailySalarySheetAsync(DateTime date);
    Task<IReadOnlyList<DailySalarySummaryVm>> GetDailySalarySummaryAsync(DateTime from, DateTime to);
}

// ViewModels referenced above - defined in area folders
public record ShiftListItemVm(int Id, string Name, TimeSpan StartTime, TimeSpan EndTime);
public record ShiftEditVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GraceMinutes { get; set; }
    public int PunchWindowBeforeMinutes { get; set; }
    public int MaxOvertimeMinutes { get; set; }
}
public record ShiftAssignmentVm(int Id, int EmployeeId, string EmployeeName, int ShiftId, string ShiftName, DateTime EffectiveFrom, DateTime? EffectiveTo);
public record PunchRecordVm(int Id, string EmployeeName, string PunchNumber, DateTime PunchTime, string PunchType, string Source);
public class PunchRecordCreateVm
{
    public int EmployeeId { get; set; }
    public DateTime PunchTime { get; set; } = DateTime.Now;
    public int PunchType { get; set; } = 1;
}
public record DailyAttendanceVm(int Id, string EmployeeName, DateTime AttendanceDate, DateTime? InTime, DateTime? OutTime, int LateMinutes, int OvertimeMinutes, bool IsAbsent, bool IsApproved);
public record AttendanceReportVm(string EmployeeCode, string EmployeeName, string Department, DateTime Date, DateTime? InTime, DateTime? OutTime, int LateMinutes, int OvertimeMinutes, bool IsAbsent);
public record AttendanceSummaryVm(string EmployeeCode, string EmployeeName, string Department, int PresentDays, int AbsentDays, int LateCount, decimal OtHours);
public record LeaveTypeVm(int Id, string Name, int MaxDaysPerYear, bool IsPaid);
public record LeaveBalanceVm(int Id, int EmployeeId, string EmployeeName, int LeaveTypeId, string LeaveTypeName, int Year, int AllocatedDays, int UsedDays, int RemainingDays);
public record LeaveApplicationVm(int Id, string EmployeeName, string LeaveTypeName, DateTime FromDate, DateTime ToDate, int TotalDays, string ApprovalStatus, string? Reason);
public record LeaveApplicationCreateVm(int EmployeeId, int LeaveTypeId, DateTime FromDate, DateTime ToDate, string? Reason);
public record PayrollPeriodVm(int Id, string Name, DateTime StartDate, DateTime EndDate, string Status);
public record PayrollDetailVm(int Id, int EmployeeId, string EmployeeName, string EmployeeCode, decimal GrossSalary, decimal BasicSalary, decimal HouseRent, decimal MedicalAllowance, decimal FoodAllowance, decimal ConveyanceAllowance, int AttendancePayableDays, decimal AbsentDeduction, decimal OvertimeAmount, decimal NightBillAmount, decimal HolidayBillAmount, decimal AdvanceDeduction, decimal NetPayableSalary);
public record PayrollSummaryVm(string Department, int EmployeeCount, decimal TotalGross, decimal TotalNet, decimal TotalOt, decimal TotalNightBill, decimal TotalHolidayBill);
public record AdvanceSalaryVm(int Id, int EmployeeId, string EmployeeName, decimal Amount, decimal MonthlyDeduction, decimal RemainingBalance, string Status, string? Reason);
public record SalaryIncrementVm(int Id, int EmployeeId, string EmployeeName, decimal PreviousGross, decimal NewGross, DateTime EffectiveDate, string? Reason);
public record BillRateConfigVm(int Id, int BillType, int RateType, decimal Amount, int? ShiftId, string? ShiftName);
