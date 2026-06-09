using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class SeparationService : ISeparationService
{
    private readonly IEmployeeSeparationRepository _separationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly AppDbContext _context;

    public SeparationService(
        IEmployeeSeparationRepository separationRepository,
        ICurrentUserService currentUser,
        AppDbContext context)
    {
        _separationRepository = separationRepository;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<IReadOnlyList<SeparationVm>> GetSeparationsAsync(SeparationFilterVm filter)
    {
        var separations = await _separationRepository.GetFilteredAsync(
            _currentUser.CompanyId, filter.From, filter.To, filter.SeparationType, filter.DepartmentId);
        return separations.Select(MapSeparation).ToList();
    }

    public Task<SeparationCreateVm?> GetCreateModelAsync(int? employeeId)
    {
        var model = new SeparationCreateVm();
        if (employeeId.HasValue) model.EmployeeId = employeeId.Value;
        return Task.FromResult<SeparationCreateVm?>(model);
    }

    public async Task<(bool Success, string? Error)> CreateSeparationAsync(SeparationCreateVm model)
    {
        if (model.EmployeeId <= 0) return (false, "Select an employee.");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == model.EmployeeId && e.CompanyId == _currentUser.CompanyId);
        if (employee == null) return (false, "Employee not found.");
        if (employee.Status != EntityStatus.Active) return (false, "Employee is already inactive.");

        var existing = await _separationRepository.GetByEmployeeIdAsync(_currentUser.CompanyId, model.EmployeeId);
        if (existing != null) return (false, "A separation record already exists for this employee.");

        await _separationRepository.AddAsync(new EmployeeSeparation
        {
            CompanyId = _currentUser.CompanyId,
            EmployeeId = model.EmployeeId,
            SeparationType = model.SeparationType,
            SeparationDate = model.SeparationDate.Date,
            NoticeDate = model.NoticeDate?.Date,
            Reason = model.Reason,
            Remarks = model.Remarks,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserName
        });

        employee.Status = EntityStatus.Inactive;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = _currentUser.UserName;

        await _separationRepository.SaveChangesAsync();
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private static SeparationVm MapSeparation(EmployeeSeparation s) => new()
    {
        Id = s.Id,
        EmployeeId = s.EmployeeId,
        EmployeeCode = s.Employee?.EmployeeCode ?? "-",
        EmployeeName = s.Employee?.FullName ?? "-",
        Department = s.Employee?.Department?.Name ?? "-",
        SeparationType = s.SeparationType,
        SeparationTypeLabel = s.SeparationType.ToString(),
        SeparationDate = s.SeparationDate,
        NoticeDate = s.NoticeDate,
        Reason = s.Reason,
        Remarks = s.Remarks,
        CreatedAt = s.CreatedAt
    };
}
