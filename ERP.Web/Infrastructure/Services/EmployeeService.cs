using ERP.Web.Areas.HR.ViewModels;
using ERP.Web.Core.DTOs;
using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;

namespace ERP.Web.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDesignationRepository _designationRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ILineRepository _lineRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAddressService _addressService;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IDesignationRepository designationRepository,
        ISectionRepository sectionRepository,
        ILineRepository lineRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAddressService addressService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _designationRepository = designationRepository;
        _sectionRepository = sectionRepository;
        _lineRepository = lineRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _addressService = addressService;
    }

    public async Task<IReadOnlyList<EmployeeListItemVm>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetByCompanyAsync(_currentUserService.CompanyId);
        return employees.Select(MapToListItem).ToList();
    }

    public async Task<IReadOnlyList<EmployeeListItemVm>> SearchAsync(EmployeeFilterDto filter)
    {
        var employees = await _employeeRepository.GetFilteredAsync(_currentUserService.CompanyId, filter);
        return employees.Select(MapToListItem).ToList();
    }

    public async Task<EmployeeDetailsVm?> GetDetailsAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(id, _currentUserService.CompanyId);
        if (employee == null) return null;
        return MapToDetails(employee);
    }

    public async Task<EmployeeEditVm?> GetForEditAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(id, _currentUserService.CompanyId);
        if (employee == null) return null;
        return MapToEdit(employee);
    }

    public async Task<(bool Success, string? Error)> CreateAsync(EmployeeCreateVm model)
    {
        var validationError = await ValidateUniqueFieldsAsync(model.EmployeeCode, model.PunchNumber);
        if (validationError != null) return (false, validationError);

        var employee = new Employee
        {
            CompanyId = _currentUserService.CompanyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserName
        };
        ApplyFormToEntity(employee, model);

        await _employeeRepository.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(EmployeeEditVm model)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(model.Id, _currentUserService.CompanyId);
        if (employee == null) return (false, "Employee not found.");

        var validationError = await ValidateUniqueFieldsAsync(model.EmployeeCode, model.PunchNumber, model.Id);
        if (validationError != null) return (false, validationError);

        ApplyFormToEntity(employee, model);
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = _currentUserService.UserName;

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(id, _currentUserService.CompanyId);
        if (employee == null) return (false, "Employee not found.");

        employee.IsDeleted = true;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = _currentUserService.UserName;
        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }

    public async Task<EmployeeFormLookupsVm> GetFormLookupsAsync()
    {
        var companyId = _currentUserService.CompanyId;
        var departments = await _departmentRepository.GetByCompanyAsync(companyId);
        var designations = await _designationRepository.GetByCompanyAsync(companyId);
        var sections = await _sectionRepository.GetByCompanyAsync(companyId);
        var lines = await _lineRepository.GetByCompanyAsync(companyId);

        return new EmployeeFormLookupsVm
        {
            Departments = departments.Select(d => new LookupItemVm { Id = d.Id, Name = d.Name }).ToList(),
            Designations = designations.Select(d => new LookupItemVm { Id = d.Id, Name = d.Title, ParentId = d.SectionId }).ToList(),
            Sections = sections.Select(s => new LookupItemVm { Id = s.Id, Name = s.Name, ParentId = s.DepartmentId }).ToList(),
            Lines = lines.Select(l => new LookupItemVm { Id = l.Id, Name = l.Name, ParentId = l.SectionId }).ToList(),
            Addresses = (await _addressService.GetAllAsync())
                .Select(a => new LookupItemVm { Id = a.Id, Name = a.DisplayName })
                .ToList()
        };
    }

    private async Task<string?> ValidateUniqueFieldsAsync(string employeeCode, string punchNumber, int? excludeId = null)
    {
        var companyId = _currentUserService.CompanyId;
        if (await _employeeRepository.EmployeeCodeExistsAsync(employeeCode.Trim(), companyId, excludeId))
            return "Employee code already exists.";
        if (await _employeeRepository.PunchNumberExistsAsync(punchNumber.Trim(), companyId, excludeId))
            return "Punch number already exists.";
        return null;
    }

    private static void ApplyFormToEntity(Employee employee, EmployeeFormVm model)
    {
        employee.EmployeeCode = model.EmployeeCode.Trim();
        employee.PunchNumber = model.PunchNumber.Trim();
        employee.FullName = model.FullName.Trim();
        employee.DateOfBirth = model.DateOfBirth;
        employee.Gender = NullIfWhiteSpace(model.Gender);
        employee.BloodGroup = NullIfWhiteSpace(model.BloodGroup);
        employee.NationalId = NullIfWhiteSpace(model.NationalId);
        employee.Phone = NullIfWhiteSpace(model.Phone);
        employee.Email = NullIfWhiteSpace(model.Email);
        employee.MaritalStatus = NullIfWhiteSpace(model.MaritalStatus);
        employee.DepartmentId = model.DepartmentId;
        employee.SectionId = model.SectionId;
        employee.LineId = model.LineId;
        employee.DesignationId = model.DesignationId;
        employee.JoiningDate = model.JoiningDate;
        employee.ConfirmationDate = model.ConfirmationDate;
        employee.Status = model.Status;
        employee.AddressId = model.AddressId;
        employee.PresentAddress = NullIfWhiteSpace(model.PresentAddress);
        employee.PermanentAddress = NullIfWhiteSpace(model.PermanentAddress);
        employee.GrossSalary = model.GrossSalary;
        employee.BasicSalary = model.BasicSalary;
        employee.HouseRentAllowance = model.HouseRentAllowance;
        employee.MedicalAllowance = model.MedicalAllowance;
        employee.TransportAllowance = model.TransportAllowance;
        employee.OtherAllowance = model.OtherAllowance;
        employee.BankName = NullIfWhiteSpace(model.BankName);
        employee.BankAccountNo = NullIfWhiteSpace(model.BankAccountNo);
        employee.BankBranch = NullIfWhiteSpace(model.BankBranch);
        employee.TaxId = NullIfWhiteSpace(model.TaxId);
        employee.FatherName = NullIfWhiteSpace(model.FatherName);
        employee.MotherName = NullIfWhiteSpace(model.MotherName);
        employee.SpouseName = NullIfWhiteSpace(model.SpouseName);
        employee.EmergencyContactName = NullIfWhiteSpace(model.EmergencyContactName);
        employee.EmergencyContactPhone = NullIfWhiteSpace(model.EmergencyContactPhone);
        employee.EmergencyContactRelation = NullIfWhiteSpace(model.EmergencyContactRelation);
        employee.SignatureData = NullIfWhiteSpace(model.SignatureData);
    }

    private static EmployeeEditVm MapToEdit(Employee employee) => new()
    {
        Id = employee.Id,
        EmployeeCode = employee.EmployeeCode,
        PunchNumber = employee.PunchNumber,
        FullName = employee.FullName,
        DateOfBirth = employee.DateOfBirth,
        Gender = employee.Gender,
        BloodGroup = employee.BloodGroup,
        NationalId = employee.NationalId,
        Phone = employee.Phone,
        Email = employee.Email,
        MaritalStatus = employee.MaritalStatus,
        DepartmentId = employee.DepartmentId,
        SectionId = employee.SectionId,
        LineId = employee.LineId,
        DesignationId = employee.DesignationId,
        JoiningDate = employee.JoiningDate,
        ConfirmationDate = employee.ConfirmationDate,
        Status = employee.Status,
        AddressId = employee.AddressId,
        PresentAddress = employee.PresentAddress,
        PermanentAddress = employee.PermanentAddress,
        GrossSalary = employee.GrossSalary,
        BasicSalary = employee.BasicSalary,
        HouseRentAllowance = employee.HouseRentAllowance,
        MedicalAllowance = employee.MedicalAllowance,
        TransportAllowance = employee.TransportAllowance,
        OtherAllowance = employee.OtherAllowance,
        BankName = employee.BankName,
        BankAccountNo = employee.BankAccountNo,
        BankBranch = employee.BankBranch,
        TaxId = employee.TaxId,
        FatherName = employee.FatherName,
        MotherName = employee.MotherName,
        SpouseName = employee.SpouseName,
        EmergencyContactName = employee.EmergencyContactName,
        EmergencyContactPhone = employee.EmergencyContactPhone,
        EmergencyContactRelation = employee.EmergencyContactRelation,
        SignatureData = employee.SignatureData
    };

    private static EmployeeDetailsVm MapToDetails(Employee employee) => new()
    {
        Id = employee.Id,
        EmployeeCode = employee.EmployeeCode,
        PunchNumber = employee.PunchNumber,
        FullName = employee.FullName,
        DateOfBirth = employee.DateOfBirth,
        Gender = employee.Gender,
        BloodGroup = employee.BloodGroup,
        NationalId = employee.NationalId,
        Phone = employee.Phone,
        Email = employee.Email,
        MaritalStatus = employee.MaritalStatus,
        DepartmentName = employee.Department?.Name ?? string.Empty,
        SectionName = employee.Section?.Name ?? string.Empty,
        LineName = employee.Line?.Name ?? string.Empty,
        DesignationTitle = employee.Designation?.Title ?? string.Empty,
        JoiningDate = employee.JoiningDate,
        ConfirmationDate = employee.ConfirmationDate,
        Status = employee.Status,
        AddressName = employee.Address == null ? null : $"{employee.Address.Label} — {employee.Address.City}",
        PresentAddress = employee.PresentAddress,
        PermanentAddress = employee.PermanentAddress,
        GrossSalary = employee.GrossSalary,
        BasicSalary = employee.BasicSalary,
        HouseRentAllowance = employee.HouseRentAllowance,
        MedicalAllowance = employee.MedicalAllowance,
        TransportAllowance = employee.TransportAllowance,
        OtherAllowance = employee.OtherAllowance,
        BankName = employee.BankName,
        BankAccountNo = employee.BankAccountNo,
        BankBranch = employee.BankBranch,
        TaxId = employee.TaxId,
        FatherName = employee.FatherName,
        MotherName = employee.MotherName,
        SpouseName = employee.SpouseName,
        EmergencyContactName = employee.EmergencyContactName,
        EmergencyContactPhone = employee.EmergencyContactPhone,
        EmergencyContactRelation = employee.EmergencyContactRelation,
        SignatureData = employee.SignatureData
    };

    private static EmployeeListItemVm MapToListItem(Employee employee) => new()
    {
        Id = employee.Id,
        EmployeeCode = employee.EmployeeCode,
        PunchNumber = employee.PunchNumber,
        FullName = employee.FullName,
        DepartmentName = employee.Department?.Name ?? string.Empty,
        SectionName = employee.Section?.Name ?? string.Empty,
        LineName = employee.Line?.Name ?? string.Empty,
        DesignationTitle = employee.Designation?.Title ?? string.Empty,
        JoiningDate = employee.JoiningDate,
        GrossSalary = employee.GrossSalary,
        Status = employee.Status
    };

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
