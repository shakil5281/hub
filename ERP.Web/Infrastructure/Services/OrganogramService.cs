using ERP.Web.Areas.Company.ViewModels;
using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;

namespace ERP.Web.Infrastructure.Services;

public class OrganogramService : IOrganogramService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ILineRepository _lineRepository;
    private readonly IDesignationRepository _designationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public OrganogramService(
        IDepartmentRepository departmentRepository,
        ISectionRepository sectionRepository,
        ILineRepository lineRepository,
        IDesignationRepository designationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _departmentRepository = departmentRepository;
        _sectionRepository = sectionRepository;
        _lineRepository = lineRepository;
        _designationRepository = designationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<OrganogramIndexVm> GetIndexDataAsync(string activeTab = "departments")
    {
        return new OrganogramIndexVm
        {
            ActiveTab = activeTab,
            Departments = await GetDepartmentsAsync(),
            Sections = await GetSectionsAsync(),
            Lines = await GetLinesAsync(),
            Designations = await GetDesignationsAsync()
        };
    }

    public async Task<IReadOnlyList<DepartmentVm>> GetDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetByCompanyAsync(_currentUserService.CompanyId);
        return departments.Select(d => new DepartmentVm { Id = d.Id, Name = d.Name, Code = d.Code }).ToList();
    }

    public async Task<DepartmentVm?> GetDepartmentAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null || department.CompanyId != _currentUserService.CompanyId) return null;
        return new DepartmentVm { Id = department.Id, Name = department.Name, Code = department.Code };
    }

    public async Task<(bool Success, string? Error)> SaveDepartmentAsync(DepartmentVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            await _departmentRepository.AddAsync(new Department
            {
                CompanyId = companyId,
                Name = model.Name.Trim(),
                Code = model.Code.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var department = await _departmentRepository.GetByIdAsync(model.Id);
            if (department == null || department.CompanyId != companyId)
                return (false, "Department not found.");

            department.Name = model.Name.Trim();
            department.Code = model.Code.Trim();
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = _currentUserService.UserName;
            _departmentRepository.Update(department);
        }

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteDepartmentAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null || department.CompanyId != _currentUserService.CompanyId)
            return (false, "Department not found.");

        department.IsDeleted = true;
        department.UpdatedAt = DateTime.UtcNow;
        department.UpdatedBy = _currentUserService.UserName;
        _departmentRepository.Update(department);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<SectionVm>> GetSectionsAsync(int? departmentId = null)
    {
        var companyId = _currentUserService.CompanyId;
        var sections = departmentId.HasValue
            ? await _sectionRepository.GetByDepartmentAsync(departmentId.Value, companyId)
            : await _sectionRepository.GetByCompanyAsync(companyId);

        return sections.Select(s => new SectionVm
        {
            Id = s.Id,
            DepartmentId = s.DepartmentId,
            DepartmentName = s.Department?.Name,
            Name = s.Name,
            Code = s.Code
        }).ToList();
    }

    public async Task<SectionVm?> GetSectionAsync(int id)
    {
        var section = await _sectionRepository.GetByIdAsync(id);
        if (section == null || section.CompanyId != _currentUserService.CompanyId) return null;
        return new SectionVm
        {
            Id = section.Id,
            DepartmentId = section.DepartmentId,
            Name = section.Name,
            Code = section.Code
        };
    }

    public async Task<(bool Success, string? Error)> SaveSectionAsync(SectionVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            await _sectionRepository.AddAsync(new Section
            {
                CompanyId = companyId,
                DepartmentId = model.DepartmentId,
                Name = model.Name.Trim(),
                Code = model.Code.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var section = await _sectionRepository.GetByIdAsync(model.Id);
            if (section == null || section.CompanyId != companyId)
                return (false, "Section not found.");

            section.DepartmentId = model.DepartmentId;
            section.Name = model.Name.Trim();
            section.Code = model.Code.Trim();
            section.UpdatedAt = DateTime.UtcNow;
            section.UpdatedBy = _currentUserService.UserName;
            _sectionRepository.Update(section);
        }

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteSectionAsync(int id)
    {
        var section = await _sectionRepository.GetByIdAsync(id);
        if (section == null || section.CompanyId != _currentUserService.CompanyId)
            return (false, "Section not found.");

        section.IsDeleted = true;
        section.UpdatedAt = DateTime.UtcNow;
        section.UpdatedBy = _currentUserService.UserName;
        _sectionRepository.Update(section);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<LineVm>> GetLinesAsync(int? sectionId = null)
    {
        var companyId = _currentUserService.CompanyId;
        var lines = sectionId.HasValue
            ? await _lineRepository.GetBySectionAsync(sectionId.Value, companyId)
            : await _lineRepository.GetByCompanyAsync(companyId);

        return lines.Select(l => new LineVm
        {
            Id = l.Id,
            SectionId = l.SectionId,
            DepartmentName = l.Section?.Department?.Name,
            SectionName = l.Section?.Name,
            Name = l.Name,
            Code = l.Code
        }).ToList();
    }

    public async Task<LineVm?> GetLineAsync(int id)
    {
        var line = await _lineRepository.GetByIdAsync(id);
        if (line == null || line.CompanyId != _currentUserService.CompanyId) return null;
        return new LineVm
        {
            Id = line.Id,
            SectionId = line.SectionId,
            Name = line.Name,
            Code = line.Code
        };
    }

    public async Task<(bool Success, string? Error)> SaveLineAsync(LineVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            await _lineRepository.AddAsync(new Line
            {
                CompanyId = companyId,
                SectionId = model.SectionId,
                Name = model.Name.Trim(),
                Code = model.Code.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var line = await _lineRepository.GetByIdAsync(model.Id);
            if (line == null || line.CompanyId != companyId)
                return (false, "Line not found.");

            line.SectionId = model.SectionId;
            line.Name = model.Name.Trim();
            line.Code = model.Code.Trim();
            line.UpdatedAt = DateTime.UtcNow;
            line.UpdatedBy = _currentUserService.UserName;
            _lineRepository.Update(line);
        }

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteLineAsync(int id)
    {
        var line = await _lineRepository.GetByIdAsync(id);
        if (line == null || line.CompanyId != _currentUserService.CompanyId)
            return (false, "Line not found.");

        line.IsDeleted = true;
        line.UpdatedAt = DateTime.UtcNow;
        line.UpdatedBy = _currentUserService.UserName;
        _lineRepository.Update(line);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<DesignationVm>> GetDesignationsAsync(int? sectionId = null)
    {
        var companyId = _currentUserService.CompanyId;
        var designations = sectionId.HasValue
            ? await _designationRepository.GetBySectionAsync(sectionId.Value, companyId)
            : await _designationRepository.GetByCompanyAsync(companyId);
        return designations.Select(MapDesignation).ToList();
    }

    public async Task<DesignationVm?> GetDesignationAsync(int id)
    {
        var designation = await _designationRepository.GetByIdAsync(id);
        if (designation == null || designation.CompanyId != _currentUserService.CompanyId) return null;

        var section = await _sectionRepository.GetByIdAsync(designation.SectionId);
        return new DesignationVm
        {
            Id = designation.Id,
            SectionId = designation.SectionId,
            DepartmentId = section?.DepartmentId ?? 0,
            Title = designation.Title,
            Code = designation.Code
        };
    }

    public async Task<(bool Success, string? Error)> SaveDesignationAsync(DesignationVm model)
    {
        var companyId = _currentUserService.CompanyId;
        var section = await _sectionRepository.GetByIdAsync(model.SectionId);
        if (section == null || section.CompanyId != companyId)
            return (false, "Section not found.");

        if (model.Id == 0)
        {
            await _designationRepository.AddAsync(new Designation
            {
                CompanyId = companyId,
                SectionId = model.SectionId,
                Title = model.Title.Trim(),
                Code = model.Code.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var designation = await _designationRepository.GetByIdAsync(model.Id);
            if (designation == null || designation.CompanyId != companyId)
                return (false, "Designation not found.");

            designation.SectionId = model.SectionId;
            designation.Title = model.Title.Trim();
            designation.Code = model.Code.Trim();
            designation.UpdatedAt = DateTime.UtcNow;
            designation.UpdatedBy = _currentUserService.UserName;
            _designationRepository.Update(designation);
        }

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteDesignationAsync(int id)
    {
        var designation = await _designationRepository.GetByIdAsync(id);
        if (designation == null || designation.CompanyId != _currentUserService.CompanyId)
            return (false, "Designation not found.");

        designation.IsDeleted = true;
        designation.UpdatedAt = DateTime.UtcNow;
        designation.UpdatedBy = _currentUserService.UserName;
        _designationRepository.Update(designation);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<OrganogramTreeVm> GetOrganogramTreeAsync()
    {
        var companyId = _currentUserService.CompanyId;
        var departments = await _departmentRepository.GetByCompanyAsync(companyId);
        var sections = await _sectionRepository.GetByCompanyAsync(companyId);
        var lines = await _lineRepository.GetByCompanyAsync(companyId);
        var designations = await _designationRepository.GetByCompanyAsync(companyId);

        var tree = new OrganogramTreeVm
        {
            Departments = departments.Select(d => new OrganogramDepartmentNode
            {
                Id = d.Id,
                Name = d.Name,
                Sections = sections
                    .Where(s => s.DepartmentId == d.Id)
                    .Select(s => new OrganogramSectionNode
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Designations = designations
                            .Where(des => des.SectionId == s.Id)
                            .Select(des => new OrganogramDesignationNode { Id = des.Id, Title = des.Title })
                            .ToList(),
                        Lines = lines
                            .Where(l => l.SectionId == s.Id)
                            .Select(l => new OrganogramLineNode { Id = l.Id, Name = l.Name })
                            .ToList()
                    })
                    .ToList()
            }).ToList()
        };

        return tree;
    }

    private static DesignationVm MapDesignation(Designation d) => new()
    {
        Id = d.Id,
        SectionId = d.SectionId,
        SectionName = d.Section?.Name,
        DepartmentName = d.Section?.Department?.Name,
        Title = d.Title,
        Code = d.Code
    };
}
