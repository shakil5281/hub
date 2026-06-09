using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Web.Infrastructure.Export;

public class PdfExportService : IPdfExportService
{
    private readonly AppDbContext _context;

    public PdfExportService(AppDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportEmployeeProfileAsync(int employeeId, int companyId)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Line)
            .Include(e => e.Designation)
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.CompanyId == companyId);

        if (employee == null) return Array.Empty<byte>();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text(employee.Company?.Name ?? "Company").Bold().FontSize(18);
                    col.Item().Text("Employee Profile").FontSize(14).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Spacing(8);
                    AddRow(col, "Employee Code", employee.EmployeeCode);
                    AddRow(col, "Punch Number", employee.PunchNumber);
                    AddRow(col, "Full Name", employee.FullName);
                    AddRow(col, "Department", employee.Department?.Name ?? "-");
                    AddRow(col, "Section", employee.Section?.Name ?? "-");
                    AddRow(col, "Line", employee.Line?.Name ?? "-");
                    AddRow(col, "Designation", employee.Designation?.Title ?? "-");
                    AddRow(col, "Joining Date", employee.JoiningDate.ToString("dd MMM yyyy"));
                    AddRow(col, "Gross Salary", employee.GrossSalary.ToString("N2"));
                    AddRow(col, "Status", employee.Status.ToString());
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated on ");
                    text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).SemiBold();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportAttendanceReportAsync(int companyId, DateTime fromDate, DateTime toDate)
    {
        var records = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date >= fromDate.Date && a.AttendanceDate.Date <= toDate.Date)
            .OrderBy(a => a.AttendanceDate).ThenBy(a => a.Employee!.EmployeeCode)
            .ToListAsync();

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Column(col =>
                {
                    col.Item().Text(company?.Name ?? "Company").Bold().FontSize(14);
                    col.Item().Text($"Attendance Report: {fromDate:dd MMM yyyy} – {toDate:dd MMM yyyy}").FontSize(10).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(55); cols.RelativeColumn(3); cols.RelativeColumn(2);
                        cols.ConstantColumn(70); cols.ConstantColumn(45); cols.ConstantColumn(45);
                        cols.ConstantColumn(40); cols.ConstantColumn(40); cols.ConstantColumn(45);
                    });
                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Date", "Employee", "Department", "Code", "In", "Out", "Late", "OT", "Absent" })
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text(h).Bold();
                    });
                    foreach (var a in records)
                    {
                        table.Cell().Padding(3).Text(a.AttendanceDate.ToString("dd/MM/yy"));
                        table.Cell().Padding(3).Text(a.Employee?.FullName);
                        table.Cell().Padding(3).Text(a.Employee?.Department?.Name);
                        table.Cell().Padding(3).Text(a.Employee?.EmployeeCode);
                        table.Cell().Padding(3).Text(a.InTime?.ToString("HH:mm") ?? "—");
                        table.Cell().Padding(3).Text(a.OutTime?.ToString("HH:mm") ?? "—");
                        table.Cell().Padding(3).Text(a.LateMinutes > 0 ? a.LateMinutes.ToString() : "—");
                        table.Cell().Padding(3).Text(a.OvertimeMinutes > 0 ? a.OvertimeMinutes.ToString() : "—");
                        table.Cell().Padding(3).Text(a.IsAbsent ? "Yes" : "No");
                    }
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Generated: ");
                    text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).SemiBold();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportPayslipAsync(int payrollDetailId, int companyId)
    {
        var detail = await _context.PayrollDetails
            .Include(d => d.Employee)
            .ThenInclude(e => e!.Department)
            .Include(d => d.PayrollSheet)
            .ThenInclude(s => s!.PayrollPeriod)
            .Include(d => d.Employee!.Company)
            .FirstOrDefaultAsync(d => d.Id == payrollDetailId && d.CompanyId == companyId);

        if (detail == null) return Array.Empty<byte>();

        var periodName = detail.PayrollSheet?.PayrollPeriod?.Name ?? "Payroll Period";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text(detail.Employee?.Company?.Name ?? "Company").Bold().FontSize(18);
                    col.Item().Text($"Payslip — {periodName}").FontSize(14).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Spacing(6);
                    AddRow(col, "Employee", $"{detail.Employee?.FullName} ({detail.Employee?.EmployeeCode})");
                    AddRow(col, "Department", detail.Employee?.Department?.Name ?? "-");
                    col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);

                    AddAmountRow(col, "Gross Salary", detail.GrossSalary);
                    AddAmountRow(col, "Basic Salary", detail.BasicSalary);
                    AddAmountRow(col, "House Rent", detail.HouseRent);
                    AddAmountRow(col, "Medical Allowance", detail.MedicalAllowance);
                    AddAmountRow(col, "Food Allowance", detail.FoodAllowance);
                    AddAmountRow(col, "Conveyance Allowance", detail.ConveyanceAllowance);
                    AddAmountRow(col, "Payable Days", detail.AttendancePayableDays, isCurrency: false);

                    col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);

                    AddAmountRow(col, "Absent Deduction", detail.AbsentDeduction, deduction: true);
                    AddAmountRow(col, "Overtime Amount", detail.OvertimeAmount);
                    AddAmountRow(col, "Night Bill", detail.NightBillAmount);
                    AddAmountRow(col, "Holiday Bill", detail.HolidayBillAmount);
                    AddAmountRow(col, "Advance Deduction", detail.AdvanceDeduction, deduction: true);

                    col.Item().PaddingTop(12).Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Text("Net Payable Salary").Bold().FontSize(13);
                        row.ConstantItem(120).AlignRight().Text(detail.NetPayableSalary.ToString("N2")).Bold().FontSize(13);
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated on ");
                    text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).SemiBold();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportMonthlySalarySheetAsync(int payrollSheetId, int companyId)
    {
        var details = await _context.PayrollDetails
            .Include(d => d.Employee).ThenInclude(e => e!.Department)
            .Include(d => d.PayrollSheet).ThenInclude(s => s!.PayrollPeriod)
            .Where(d => d.PayrollSheetId == payrollSheetId && d.CompanyId == companyId)
            .OrderBy(d => d.Employee!.EmployeeCode)
            .ToListAsync();

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
        var periodName = details.FirstOrDefault()?.PayrollSheet?.PayrollPeriod?.Name ?? string.Empty;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Column(col =>
                {
                    col.Item().Text(company?.Name ?? "Company").Bold().FontSize(14);
                    col.Item().Text($"Salary Sheet — {periodName}").FontSize(11).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(50); cols.RelativeColumn(3); cols.RelativeColumn(2);
                        cols.RelativeColumn(2); cols.RelativeColumn(2); cols.RelativeColumn(2);
                        cols.RelativeColumn(2); cols.RelativeColumn(2); cols.RelativeColumn(2);
                    });
                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Code", "Name", "Department", "Gross", "Basic", "Absent Ded.", "OT", "Advance Ded.", "Net Pay" })
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text(h).Bold();
                    });
                    foreach (var d in details)
                    {
                        table.Cell().Padding(3).Text(d.Employee?.EmployeeCode);
                        table.Cell().Padding(3).Text(d.Employee?.FullName);
                        table.Cell().Padding(3).Text(d.Employee?.Department?.Name);
                        table.Cell().Padding(3).AlignRight().Text(d.GrossSalary.ToString("N2"));
                        table.Cell().Padding(3).AlignRight().Text(d.BasicSalary.ToString("N2"));
                        table.Cell().Padding(3).AlignRight().Text(d.AbsentDeduction.ToString("N2"));
                        table.Cell().Padding(3).AlignRight().Text(d.OvertimeAmount.ToString("N2"));
                        table.Cell().Padding(3).AlignRight().Text(d.AdvanceDeduction.ToString("N2"));
                        table.Cell().Padding(3).AlignRight().Text(d.NetPayableSalary.ToString("N2"));
                    }
                });

                var totalNet = details.Sum(d => d.NetPayableSalary);
                page.Footer().Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text($"Employees: {details.Count}  |  Total Net Payable: {totalNet:N2}").SemiBold();
                    row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");
                });
            });
        });

        return document.GeneratePdf();
    }

    private static void AddRow(ColumnDescriptor col, string label, string value)
    {
        col.Item().Row(row =>
        {
            row.ConstantItem(140).Text(label).SemiBold();
            row.RelativeItem().Text(value);
        });
    }

    private static void AddAmountRow(ColumnDescriptor col, string label, decimal amount, bool deduction = false, bool isCurrency = true)
    {
        var display = isCurrency ? (deduction && amount > 0 ? $"-{amount:N2}" : amount.ToString("N2")) : amount.ToString();
        AddRow(col, label, display);
    }

    public async Task<byte[]> ExportJobCardPdfAsync(int companyId, DateTime date)
    {
        var cards = await _context.JobCards
            .Include(j => j.Employee).ThenInclude(e => e!.Department)
            .Include(j => j.Line)
            .Where(j => j.CompanyId == companyId && j.WorkDate.Date == date.Date)
            .OrderBy(j => j.Employee!.EmployeeCode)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));
                page.Header().Text($"Job Card Report — {date:dd MMM yyyy}").Bold().FontSize(14);
                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(1); cols.RelativeColumn(3); cols.RelativeColumn(2);
                        cols.RelativeColumn(2); cols.RelativeColumn(2); cols.RelativeColumn(2); cols.RelativeColumn(1);
                    });
                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Code", "Name", "Department", "Line", "Target", "Achieved", "%" })
                            header.Cell().Padding(4).Text(h).Bold();
                    });
                    foreach (var j in cards)
                    {
                        table.Cell().Padding(4).Text(j.Employee?.EmployeeCode);
                        table.Cell().Padding(4).Text(j.Employee?.FullName);
                        table.Cell().Padding(4).Text(j.Employee?.Department?.Name);
                        table.Cell().Padding(4).Text(j.Line?.Name);
                        table.Cell().Padding(4).Text(j.TargetQty.ToString("N1"));
                        table.Cell().Padding(4).Text(j.AchievedQty.ToString("N1"));
                        var eff = j.TargetQty > 0 ? Math.Round(j.AchievedQty / j.TargetQty * 100, 1) : 0;
                        table.Cell().Padding(4).Text($"{eff}%");
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportMonthlyReportPdfAsync(int companyId, int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);

        var attendances = await _context.DailyAttendances
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .Where(a => a.CompanyId == companyId && a.AttendanceDate >= from && a.AttendanceDate <= to)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Header().Column(col =>
                {
                    col.Item().Text(company?.Name ?? "Company").Bold().FontSize(16);
                    col.Item().Text($"Monthly Report — {from:MMMM yyyy}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });
                page.Content().PaddingTop(10).Column(col =>
                {
                    col.Spacing(6);
                    var deptGroups = attendances.GroupBy(a => a.Employee?.Department?.Name ?? "Unassigned").OrderBy(g => g.Key);
                    foreach (var g in deptGroups)
                    {
                        col.Item().Background(Colors.Grey.Lighten3).Padding(6).Text(g.Key).Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1); });
                        table.Header(header =>
                        {
                            foreach (var h in new[] { "Metric", "Present", "Absent", "OT Hours" })
                                header.Cell().Padding(4).Text(h).Bold();
                        });
                        table.Cell().Padding(4).Text("Count");
                        table.Cell().Padding(4).Text(g.Count(a => !a.IsAbsent && a.InTime.HasValue).ToString());
                        table.Cell().Padding(4).Text(g.Count(a => a.IsAbsent).ToString());
                        table.Cell().Padding(4).Text(Math.Round(g.Sum(a => a.OvertimeMinutes) / 60m, 2).ToString("N2"));
                    });
                    }
                });
            });
        });

        return document.GeneratePdf();
    }
}
