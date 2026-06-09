using ERP.Web.Core.Enums;

namespace ERP.Web.Core.DTOs;

public class EmployeeFilterDto
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public EntityStatus? Status { get; set; }
    public DateTime? JoiningFrom { get; set; }
    public DateTime? JoiningTo { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}

public class AttendanceFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public string? Search { get; set; }
    public bool? IsAbsent { get; set; }
    public bool? IsApproved { get; set; }
    public bool? HasLate { get; set; }
    public bool? HasOvertime { get; set; }
}

public class PunchFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public string? Search { get; set; }
    public PunchSource? Source { get; set; }
}

public class PayrollFilterDto
{
    public int? PeriodId { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public string? Search { get; set; }
    public decimal? MinNet { get; set; }
    public decimal? MaxNet { get; set; }
}

public class LeaveFilterDto
{
    public int? EmployeeId { get; set; }
    public int? LeaveTypeId { get; set; }
    public LeaveApprovalStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Search { get; set; }
}

public class UserFilterDto
{
    public string? Search { get; set; }
    public string? RoleName { get; set; }
    public bool? IsActive { get; set; }
}
