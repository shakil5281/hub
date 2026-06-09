using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class ManpowerRequirementService : IManpowerRequirementService
{
    private readonly IStaffingPlanRepository _staffingPlanRepository;
    private readonly IHiringRequestRepository _hiringRequestRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly AppDbContext _context;

    public ManpowerRequirementService(
        IStaffingPlanRepository staffingPlanRepository,
        IHiringRequestRepository hiringRequestRepository,
        ICurrentUserService currentUser,
        AppDbContext context)
    {
        _staffingPlanRepository = staffingPlanRepository;
        _hiringRequestRepository = hiringRequestRepository;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<IReadOnlyList<StaffingPlanVm>> GetStaffingPlansAsync(int year, int month)
    {
        var plans = await _staffingPlanRepository.GetByPeriodAsync(_currentUser.CompanyId, year, month);
        return plans.Select(MapStaffingPlan).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveStaffingPlanAsync(StaffingPlanVm model)
    {
        if (model.RequiredCount < 0) return (false, "Required count cannot be negative.");

        var duplicate = await _staffingPlanRepository.GetDuplicateAsync(
            _currentUser.CompanyId, model.DepartmentId, model.SectionId, model.LineId, model.DesignationId,
            model.Year, model.Month, model.Id > 0 ? model.Id : null);
        if (duplicate != null) return (false, "A staffing plan already exists for this org unit and period.");

        if (model.Id > 0)
        {
            var existing = await _staffingPlanRepository.GetByIdAsync(model.Id, _currentUser.CompanyId);
            if (existing == null) return (false, "Staffing plan not found.");
            existing.DepartmentId = model.DepartmentId;
            existing.SectionId = model.SectionId;
            existing.LineId = model.LineId;
            existing.DesignationId = model.DesignationId;
            existing.Year = model.Year;
            existing.Month = model.Month;
            existing.RequiredCount = model.RequiredCount;
            existing.Remarks = model.Remarks;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUser.UserName;
        }
        else
        {
            await _staffingPlanRepository.AddAsync(new StaffingPlan
            {
                CompanyId = _currentUser.CompanyId,
                DepartmentId = model.DepartmentId,
                SectionId = model.SectionId,
                LineId = model.LineId,
                DesignationId = model.DesignationId,
                Year = model.Year,
                Month = model.Month,
                RequiredCount = model.RequiredCount,
                Remarks = model.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName
            });
        }

        await _staffingPlanRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteStaffingPlanAsync(int id)
    {
        var plan = await _staffingPlanRepository.GetByIdAsync(id, _currentUser.CompanyId);
        if (plan == null) return (false, "Staffing plan not found.");
        plan.IsDeleted = true;
        plan.UpdatedAt = DateTime.UtcNow;
        plan.UpdatedBy = _currentUser.UserName;
        await _staffingPlanRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<HiringRequestVm>> GetHiringRequestsAsync(HiringRequestStatus? status)
    {
        var requests = await _hiringRequestRepository.GetAllAsync(_currentUser.CompanyId, status);
        return requests.Select(MapHiringRequest).ToList();
    }

    public async Task<(bool Success, string? Error)> SaveHiringRequestAsync(HiringRequestVm model, bool submit)
    {
        if (model.RequestedCount <= 0) return (false, "Requested count must be greater than zero.");

        if (model.Id > 0)
        {
            var existing = await _hiringRequestRepository.GetByIdAsync(model.Id, _currentUser.CompanyId);
            if (existing == null) return (false, "Hiring request not found.");
            if (existing.HiringRequestStatus is HiringRequestStatus.Approved or HiringRequestStatus.Rejected or HiringRequestStatus.Filled or HiringRequestStatus.Cancelled)
                return (false, "Cannot edit a finalized request.");

            existing.DepartmentId = model.DepartmentId;
            existing.SectionId = model.SectionId;
            existing.LineId = model.LineId;
            existing.DesignationId = model.DesignationId;
            existing.RequestedCount = model.RequestedCount;
            existing.RequestDate = model.RequestDate;
            existing.TargetJoinDate = model.TargetJoinDate;
            existing.Reason = model.Reason;
            if (submit) existing.HiringRequestStatus = HiringRequestStatus.Pending;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = _currentUser.UserName;
        }
        else
        {
            await _hiringRequestRepository.AddAsync(new HiringRequest
            {
                CompanyId = _currentUser.CompanyId,
                DepartmentId = model.DepartmentId,
                SectionId = model.SectionId,
                LineId = model.LineId,
                DesignationId = model.DesignationId,
                RequestedCount = model.RequestedCount,
                RequestDate = model.RequestDate,
                TargetJoinDate = model.TargetJoinDate,
                Reason = model.Reason,
                HiringRequestStatus = submit ? HiringRequestStatus.Pending : HiringRequestStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName
            });
        }

        await _hiringRequestRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ApproveHiringRequestAsync(int id, bool approved)
    {
        var request = await _hiringRequestRepository.GetByIdAsync(id, _currentUser.CompanyId);
        if (request == null) return (false, "Hiring request not found.");
        if (request.HiringRequestStatus != HiringRequestStatus.Pending)
            return (false, "Only pending requests can be approved or rejected.");

        request.HiringRequestStatus = approved ? HiringRequestStatus.Approved : HiringRequestStatus.Rejected;
        request.ApprovedDate = DateTime.UtcNow;
        request.ApprovedBy = _currentUser.UserName;
        request.UpdatedAt = DateTime.UtcNow;
        request.UpdatedBy = _currentUser.UserName;
        await _hiringRequestRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<VacancyReportVm>> GetVacancyReportAsync(int year, int month)
    {
        var companyId = _currentUser.CompanyId;
        var plans = await _staffingPlanRepository.GetByPeriodAsync(companyId, year, month);

        var employees = await _context.Employees
            .Include(e => e.Department).Include(e => e.Section).Include(e => e.Line).Include(e => e.Designation)
            .Where(e => e.CompanyId == companyId && e.Status == EntityStatus.Active)
            .ToListAsync();

        var approvedHiring = await _hiringRequestRepository.GetApprovedForPeriodAsync(companyId, year, month);

        return plans.Select(plan =>
        {
            var actual = employees.Count(e =>
                e.DepartmentId == plan.DepartmentId &&
                (plan.SectionId == null || e.SectionId == plan.SectionId) &&
                (plan.LineId == null || e.LineId == plan.LineId) &&
                (plan.DesignationId == null || e.DesignationId == plan.DesignationId));

            var incoming = approvedHiring
                .Where(h =>
                    h.DepartmentId == plan.DepartmentId &&
                    (plan.SectionId == null || h.SectionId == plan.SectionId) &&
                    (plan.LineId == null || h.LineId == plan.LineId) &&
                    (plan.DesignationId == null || h.DesignationId == plan.DesignationId))
                .Sum(h => h.RequestedCount);

            var vacancy = Math.Max(0, plan.RequiredCount - actual);

            return new VacancyReportVm
            {
                Department = plan.Department?.Name ?? "-",
                Section = plan.Section?.Name ?? "-",
                Line = plan.Line?.Name ?? "-",
                Designation = plan.Designation?.Title ?? "-",
                RequiredCount = plan.RequiredCount,
                ActualCount = actual,
                IncomingCount = incoming,
                VacancyCount = vacancy
            };
        }).ToList();
    }

    private static StaffingPlanVm MapStaffingPlan(StaffingPlan p) => new()
    {
        Id = p.Id,
        DepartmentId = p.DepartmentId,
        SectionId = p.SectionId,
        LineId = p.LineId,
        DesignationId = p.DesignationId,
        Year = p.Year,
        Month = p.Month,
        RequiredCount = p.RequiredCount,
        Remarks = p.Remarks,
        Department = p.Department?.Name ?? "-",
        Section = p.Section?.Name ?? "-",
        Line = p.Line?.Name ?? "-",
        Designation = p.Designation?.Title ?? "-"
    };

    private static HiringRequestVm MapHiringRequest(HiringRequest h) => new()
    {
        Id = h.Id,
        DepartmentId = h.DepartmentId,
        SectionId = h.SectionId,
        LineId = h.LineId,
        DesignationId = h.DesignationId,
        RequestedCount = h.RequestedCount,
        RequestDate = h.RequestDate,
        TargetJoinDate = h.TargetJoinDate,
        Reason = h.Reason,
        HiringRequestStatus = h.HiringRequestStatus,
        Department = h.Department?.Name ?? "-",
        Section = h.Section?.Name ?? "-",
        Line = h.Line?.Name ?? "-",
        Designation = h.Designation?.Title ?? "-",
        StatusLabel = h.HiringRequestStatus.ToString()
    };
}
