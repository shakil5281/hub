using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string PunchNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? MaritalStatus { get; set; }

    public int DepartmentId { get; set; }
    public int SectionId { get; set; }
    public int LineId { get; set; }
    public int DesignationId { get; set; }
    public DateTime JoiningDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }

    public int? AddressId { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }

    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HouseRentAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowance { get; set; }

    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankBranch { get; set; }
    public string? TaxId { get; set; }

    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? SpouseName { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelation { get; set; }

    public string? SignatureData { get; set; }

    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public Line? Line { get; set; }
    public Designation? Designation { get; set; }
    public CompanyAddress? Address { get; set; }
    public Company? Company { get; set; }
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
    public ICollection<PunchRecord> PunchRecords { get; set; } = new List<PunchRecord>();
    public ICollection<DailyAttendance> DailyAttendances { get; set; } = new List<DailyAttendance>();
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    public ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
    public ICollection<AdvanceSalary> AdvanceSalaries { get; set; } = new List<AdvanceSalary>();
    public ICollection<SalaryIncrement> SalaryIncrements { get; set; } = new List<SalaryIncrement>();
}
