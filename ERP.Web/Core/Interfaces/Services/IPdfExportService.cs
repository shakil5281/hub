namespace ERP.Web.Core.Interfaces.Services;

public interface IPdfExportService
{
    Task<byte[]> ExportEmployeeProfileAsync(int employeeId, int companyId);
    Task<byte[]> ExportAttendanceReportAsync(int companyId, DateTime fromDate, DateTime toDate);
    Task<byte[]> ExportPayslipAsync(int payrollDetailId, int companyId);
    Task<byte[]> ExportMonthlySalarySheetAsync(int payrollSheetId, int companyId);
    Task<byte[]> ExportJobCardPdfAsync(int companyId, DateTime date);
    Task<byte[]> ExportMonthlyReportPdfAsync(int companyId, int year, int month);
}
