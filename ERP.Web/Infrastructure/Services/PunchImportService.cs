using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ERP.Web.Infrastructure.Services;

public class PunchImportService : IPunchImportService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PunchImportService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<(int Imported, IList<string> Errors)> ImportAsync(Stream stream)
    {
        var errors = new List<string>();
        var imported = 0;
        var companyId = _currentUserService.CompanyId;

        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .ToDictionaryAsync(e => e.PunchNumber, e => e.Id, StringComparer.OrdinalIgnoreCase);

        using var package = new ExcelPackage(stream);
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
            var punchNumber = GetCellText(worksheet, row, headerMap, "punchnumber", "punch number", "punch_no");
            var punchTimeText = GetCellText(worksheet, row, headerMap, "punchtime", "punch time", "datetime", "date time");
            var punchTypeText = GetCellText(worksheet, row, headerMap, "punchtype", "punch type", "type");

            if (string.IsNullOrWhiteSpace(punchNumber) && string.IsNullOrWhiteSpace(punchTimeText))
                continue;

            if (string.IsNullOrWhiteSpace(punchNumber))
            {
                errors.Add($"Row {row}: Punch number is required.");
                continue;
            }

            if (!employees.TryGetValue(punchNumber.Trim(), out var employeeId))
            {
                errors.Add($"Row {row}: Employee with punch number '{punchNumber}' not found.");
                continue;
            }

            if (!DateTime.TryParse(punchTimeText, out var punchTime))
            {
                errors.Add($"Row {row}: Invalid punch time '{punchTimeText}'.");
                continue;
            }

            var punchType = ParsePunchType(punchTypeText);

            _context.PunchRecords.Add(new PunchRecord
            {
                CompanyId = companyId,
                EmployeeId = employeeId,
                PunchTime = punchTime,
                PunchType = punchType,
                Source = PunchSource.Import,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });

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
            if (!string.IsNullOrEmpty(header))
                map[header.Replace(" ", "").ToLowerInvariant()] = col;
            map[header?.Trim().ToLowerInvariant() ?? string.Empty] = col;
        }
        return map;
    }

    private static string GetCellText(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMap, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (headerMap.TryGetValue(key.Replace(" ", "").ToLowerInvariant(), out var col))
                return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
            if (headerMap.TryGetValue(key.ToLowerInvariant(), out col))
                return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }

    private static PunchType ParsePunchType(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return PunchType.In;
        var normalized = text.Trim().ToLowerInvariant();
        return normalized is "out" or "2" or "o" ? PunchType.Out : PunchType.In;
    }
}
