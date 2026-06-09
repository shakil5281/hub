using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ShiftService(
        IShiftRepository shiftRepository,
        IUnitOfWork unitOfWork,
        AppDbContext context,
        ICurrentUserService currentUserService)
    {
        _shiftRepository = shiftRepository;
        _unitOfWork = unitOfWork;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<ShiftListItemVm>> GetAllAsync()
    {
        var shifts = await _shiftRepository.GetByCompanyAsync(_currentUserService.CompanyId);
        return shifts.Select(s => new ShiftListItemVm(s.Id, s.Name, s.StartTime, s.EndTime)).ToList();
    }

    public async Task<ShiftEditVm?> GetForEditAsync(int id)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        if (shift == null || shift.CompanyId != _currentUserService.CompanyId) return null;

        return new ShiftEditVm
        {
            Id = shift.Id,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            GraceMinutes = shift.GraceMinutes,
            PunchWindowBeforeMinutes = shift.PunchWindowBeforeMinutes,
            MaxOvertimeMinutes = shift.MaxOvertimeMinutes
        };
    }

    public async Task<(bool Success, string? Error)> SaveAsync(ShiftEditVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            await _shiftRepository.AddAsync(new Shift
            {
                CompanyId = companyId,
                Name = model.Name.Trim(),
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                GraceMinutes = model.GraceMinutes,
                PunchWindowBeforeMinutes = model.PunchWindowBeforeMinutes,
                MaxOvertimeMinutes = model.MaxOvertimeMinutes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var shift = await _shiftRepository.GetByIdAsync(model.Id);
            if (shift == null || shift.CompanyId != companyId)
                return (false, "Shift not found.");

            shift.Name = model.Name.Trim();
            shift.StartTime = model.StartTime;
            shift.EndTime = model.EndTime;
            shift.GraceMinutes = model.GraceMinutes;
            shift.PunchWindowBeforeMinutes = model.PunchWindowBeforeMinutes;
            shift.MaxOvertimeMinutes = model.MaxOvertimeMinutes;
            shift.UpdatedAt = DateTime.UtcNow;
            shift.UpdatedBy = _currentUserService.UserName;
            _shiftRepository.Update(shift);
        }

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var shift = await _shiftRepository.GetByIdAsync(id);
        if (shift == null || shift.CompanyId != _currentUserService.CompanyId)
            return (false, "Shift not found.");

        shift.IsDeleted = true;
        shift.UpdatedAt = DateTime.UtcNow;
        shift.UpdatedBy = _currentUserService.UserName;
        _shiftRepository.Update(shift);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<ShiftAssignmentVm>> GetAssignmentsAsync()
    {
        return await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.Shift)
            .Where(a => a.CompanyId == _currentUserService.CompanyId)
            .OrderByDescending(a => a.EffectiveFrom)
            .Select(a => new ShiftAssignmentVm(
                a.Id,
                a.EmployeeId,
                a.Employee!.FullName,
                a.ShiftId,
                a.Shift!.Name,
                a.EffectiveFrom,
                a.EffectiveTo))
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> SaveAssignmentAsync(ShiftAssignmentVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            _context.ShiftAssignments.Add(new ShiftAssignment
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                ShiftId = model.ShiftId,
                EffectiveFrom = model.EffectiveFrom.Date,
                EffectiveTo = model.EffectiveTo?.Date,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var assignment = await _context.ShiftAssignments
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.CompanyId == companyId);
            if (assignment == null) return (false, "Assignment not found.");

            assignment.EmployeeId = model.EmployeeId;
            assignment.ShiftId = model.ShiftId;
            assignment.EffectiveFrom = model.EffectiveFrom.Date;
            assignment.EffectiveTo = model.EffectiveTo?.Date;
            assignment.UpdatedAt = DateTime.UtcNow;
            assignment.UpdatedBy = _currentUserService.UserName;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }
}
