using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class PayrollDetail : BaseEntity
{
    public int PayrollSheetId { get; set; }
    public int EmployeeId { get; set; }
    public int? SalaryStructureId { get; set; }
    public int? MonthlyAttendanceSummaryId { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal FoodAllowance { get; set; }
    public decimal ConveyanceAllowance { get; set; }
    public int AttendancePayableDays { get; set; }
    public decimal AbsentDeduction { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal NightBillAmount { get; set; }
    public decimal HolidayBillAmount { get; set; }
    public decimal WeeklyOffBillAmount { get; set; }
    public decimal AttendanceBonusAmount { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal IncrementAdjustment { get; set; }
    public decimal NetPayableSalary { get; set; }
    public string? DetailStatus { get; set; }

    public PayrollSheet? PayrollSheet { get; set; }
    public Employee? Employee { get; set; }
    public EmployeeSalaryStructure? SalaryStructure { get; set; }
    public MonthlyAttendanceSummary? MonthlyAttendanceSummary { get; set; }
}
