using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ERP.Web.Infrastructure.Export;

public class ExcelImportService : IExcelImportService
{
    private readonly AppDbContext _context;

    public ExcelImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int Imported, IList<string> Errors)> ImportEmployeesAsync(Stream fileStream, int companyId, string userId)
    {
        var errors = new List<string>();
        var imported = 0;

        var departments = await _context.Departments.Where(d => d.CompanyId == companyId).ToListAsync();
        var sections = await _context.Sections.Where(s => s.CompanyId == companyId).ToListAsync();
        var lines = await _context.Lines.Where(l => l.CompanyId == companyId).ToListAsync();
        var designations = await _context.Designations.Where(d => d.CompanyId == companyId).ToListAsync();
        var existingCodes = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => e.EmployeeCode)
            .ToListAsync();
        var existingPunches = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => e.PunchNumber)
            .ToListAsync();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            errors.Add("No worksheet found in the file.");
            return (0, errors);
        }

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            errors.Add("No data rows found.");
            return (0, errors);
        }

        var headerMap = BuildHeaderMap(worksheet, 1);

        for (var row = 2; row <= rowCount; row++)
        {
            var employeeCode = GetCellText(worksheet, row, headerMap, "employeecode", "employee code", "code");
            var punchNumber = GetCellText(worksheet, row, headerMap, "punchnumber", "punch number");
            var fullName = GetCellText(worksheet, row, headerMap, "fullname", "full name", "name");
            var departmentName = GetCellText(worksheet, row, headerMap, "department");
            var sectionName = GetCellText(worksheet, row, headerMap, "section");
            var lineName = GetCellText(worksheet, row, headerMap, "line");
            var designationName = GetCellText(worksheet, row, headerMap, "designation", "title");
            var joiningDateText = GetCellText(worksheet, row, headerMap, "joiningdate", "joining date");
            var grossText = GetCellText(worksheet, row, headerMap, "grosssalary", "gross salary", "gross");
            var statusText = GetCellText(worksheet, row, headerMap, "status");

            if (string.IsNullOrWhiteSpace(employeeCode) && string.IsNullOrWhiteSpace(fullName))
                continue;

            if (string.IsNullOrWhiteSpace(employeeCode) || string.IsNullOrWhiteSpace(punchNumber) || string.IsNullOrWhiteSpace(fullName))
            {
                errors.Add($"Row {row}: Employee code, punch number, and full name are required.");
                continue;
            }

            if (existingCodes.Contains(employeeCode, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Row {row}: Employee code '{employeeCode}' already exists.");
                continue;
            }

            if (existingPunches.Contains(punchNumber, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Row {row}: Punch number '{punchNumber}' already exists.");
                continue;
            }

            var department = departments.FirstOrDefault(d => d.Name.Equals(departmentName, StringComparison.OrdinalIgnoreCase));
            var section = sections.FirstOrDefault(s => s.Name.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var line = lines.FirstOrDefault(l => l.Name.Equals(lineName, StringComparison.OrdinalIgnoreCase));
            var designation = designations.FirstOrDefault(d =>
                d.Title.Equals(designationName, StringComparison.OrdinalIgnoreCase)
                || d.Code.Equals(designationName, StringComparison.OrdinalIgnoreCase));

            if (department == null || section == null || line == null || designation == null)
            {
                errors.Add($"Row {row}: Invalid department, section, line, or designation lookup.");
                continue;
            }

            if (!DateTime.TryParse(joiningDateText, out var joiningDate))
                joiningDate = DateTime.Today;

            if (!decimal.TryParse(grossText, out var grossSalary))
                grossSalary = 0;

            var status = Enum.TryParse<EntityStatus>(statusText, true, out var parsedStatus)
                ? parsedStatus
                : EntityStatus.Active;

            _context.Employees.Add(new Employee
            {
                CompanyId = companyId,
                EmployeeCode = employeeCode.Trim(),
                PunchNumber = punchNumber.Trim(),
                FullName = fullName.Trim(),
                DepartmentId = department.Id,
                SectionId = section.Id,
                LineId = line.Id,
                DesignationId = designation.Id,
                JoiningDate = joiningDate.Date,
                GrossSalary = grossSalary,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            });

            existingCodes.Add(employeeCode);
            existingPunches.Add(punchNumber);
            imported++;
        }

        if (imported > 0)
            await _context.SaveChangesAsync();

        return (imported, errors);
    }

    private static Dictionary<string, int> BuildHeaderMap(ExcelWorksheet worksheet, int headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var colCount = worksheet.Dimension?.Columns ?? 0;
        for (var col = 1; col <= colCount; col++)
        {
            var header = worksheet.Cells[headerRow, col].Text?.Trim();
            if (string.IsNullOrEmpty(header)) continue;
            map[header] = col;
            map[header.Replace(" ", "").ToLowerInvariant()] = col;
        }
        return map;
    }

    private static string GetCellText(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMap, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (headerMap.TryGetValue(key, out var col))
                return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
            var normalized = key.Replace(" ", "").ToLowerInvariant();
            if (headerMap.TryGetValue(normalized, out col))
                return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }
}
