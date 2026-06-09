using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Interfaces.Services;

public interface IManpowerRequirementService
{
    Task<IReadOnlyList<StaffingPlanVm>> GetStaffingPlansAsync(int year, int month);
    Task<(bool Success, string? Error)> SaveStaffingPlanAsync(StaffingPlanVm model);
    Task<(bool Success, string? Error)> DeleteStaffingPlanAsync(int id);
    Task<IReadOnlyList<HiringRequestVm>> GetHiringRequestsAsync(HiringRequestStatus? status);
    Task<(bool Success, string? Error)> SaveHiringRequestAsync(HiringRequestVm model, bool submit);
    Task<(bool Success, string? Error)> ApproveHiringRequestAsync(int id, bool approved);
    Task<IReadOnlyList<VacancyReportVm>> GetVacancyReportAsync(int year, int month);
}

public interface ISeparationService
{
    Task<IReadOnlyList<SeparationVm>> GetSeparationsAsync(SeparationFilterVm filter);
    Task<SeparationCreateVm?> GetCreateModelAsync(int? employeeId);
    Task<(bool Success, string? Error)> CreateSeparationAsync(SeparationCreateVm model);
}

public class StaffingPlanVm
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int RequiredCount { get; set; }
    public string? Remarks { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

public class HiringRequestVm
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
    public int? DesignationId { get; set; }
    public int RequestedCount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? TargetJoinDate { get; set; }
    public string? Reason { get; set; }
    public HiringRequestStatus HiringRequestStatus { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
}

public class VacancyReportVm
{
    public string Department { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int RequiredCount { get; set; }
    public int ActualCount { get; set; }
    public int IncomingCount { get; set; }
    public int VacancyCount { get; set; }
}

public class SeparationVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public SeparationType SeparationType { get; set; }
    public string SeparationTypeLabel { get; set; } = string.Empty;
    public DateTime SeparationDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    public string? Reason { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SeparationCreateVm
{
    public int EmployeeId { get; set; }
    public SeparationType SeparationType { get; set; } = SeparationType.Resignation;
    public DateTime SeparationDate { get; set; } = DateTime.Today;
    public DateTime? NoticeDate { get; set; }
    public string? Reason { get; set; }
    public string? Remarks { get; set; }
}

public class SeparationFilterVm
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public SeparationType? SeparationType { get; set; }
    public int? DepartmentId { get; set; }
}
