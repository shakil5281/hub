using System.ComponentModel.DataAnnotations;
using ERP.Web.Core.Enums;

namespace ERP.Web.Areas.HR.ViewModels;

public class EmployeeListItemVm
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string PunchNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public string DesignationTitle { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public decimal GrossSalary { get; set; }
    public EntityStatus Status { get; set; }
}

public class EmployeeFormVm
{
    [Required, StringLength(20)]
    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "Punch Number")]
    public string PunchNumber { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Employee Name")]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    [Display(Name = "Gender")]
    public string? Gender { get; set; }

    [StringLength(10)]
    [Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    [StringLength(50)]
    [Display(Name = "National ID")]
    public string? NationalId { get; set; }

    [StringLength(30)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [StringLength(150), EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Display(Name = "Marital Status")]
    public string? MaritalStatus { get; set; }

    [Required]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Required]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required]
    [Display(Name = "Line")]
    public int LineId { get; set; }

    [Required]
    [Display(Name = "Designation")]
    public int DesignationId { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Confirmation Date")]
    public DateTime? ConfirmationDate { get; set; }

    [Display(Name = "Status")]
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    [Display(Name = "Registered Address")]
    public int? AddressId { get; set; }

    [StringLength(500)]
    [Display(Name = "Present Address")]
    public string? PresentAddress { get; set; }

    [StringLength(500)]
    [Display(Name = "Permanent Address")]
    public string? PermanentAddress { get; set; }

    [Required, Range(0, double.MaxValue)]
    [Display(Name = "Gross Salary")]
    public decimal GrossSalary { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Basic Salary")]
    public decimal BasicSalary { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "House Rent Allowance")]
    public decimal HouseRentAllowance { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Medical Allowance")]
    public decimal MedicalAllowance { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Transport Allowance")]
    public decimal TransportAllowance { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Other Allowance")]
    public decimal OtherAllowance { get; set; }

    [StringLength(100)]
    [Display(Name = "Bank Name")]
    public string? BankName { get; set; }

    [StringLength(50)]
    [Display(Name = "Account Number")]
    public string? BankAccountNo { get; set; }

    [StringLength(100)]
    [Display(Name = "Branch")]
    public string? BankBranch { get; set; }

    [StringLength(50)]
    [Display(Name = "Tax ID / TIN")]
    public string? TaxId { get; set; }

    [StringLength(150)]
    [Display(Name = "Father's Name")]
    public string? FatherName { get; set; }

    [StringLength(150)]
    [Display(Name = "Mother's Name")]
    public string? MotherName { get; set; }

    [StringLength(150)]
    [Display(Name = "Spouse Name")]
    public string? SpouseName { get; set; }

    [StringLength(150)]
    [Display(Name = "Emergency Contact Name")]
    public string? EmergencyContactName { get; set; }

    [StringLength(30)]
    [Display(Name = "Emergency Contact Phone")]
    public string? EmergencyContactPhone { get; set; }

    [StringLength(50)]
    [Display(Name = "Relation")]
    public string? EmergencyContactRelation { get; set; }

    [Display(Name = "Signature")]
    public string? SignatureData { get; set; }
}

public class EmployeeCreateVm : EmployeeFormVm;

public class EmployeeEditVm : EmployeeFormVm
{
    public int Id { get; set; }
}

public class EmployeeDetailsVm
{
    public int Id { get; set; }
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

    public string DepartmentName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public string DesignationTitle { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public EntityStatus Status { get; set; }

    public string? AddressName { get; set; }
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
}

public class EmployeeFormLookupsVm
{
    public IList<LookupItemVm> Departments { get; set; } = new List<LookupItemVm>();
    public IList<LookupItemVm> Sections { get; set; } = new List<LookupItemVm>();
    public IList<LookupItemVm> Lines { get; set; } = new List<LookupItemVm>();
    public IList<LookupItemVm> Designations { get; set; } = new List<LookupItemVm>();
    public IList<LookupItemVm> Addresses { get; set; } = new List<LookupItemVm>();
}

public class LookupItemVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}
