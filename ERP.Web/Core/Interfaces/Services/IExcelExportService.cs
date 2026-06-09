using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Interfaces.Services;

public interface IExcelExportService
{
    Task<byte[]> ExportEmployeesAsync(int companyId);
    Task<byte[]> ExportDailyAttendanceAsync(int companyId, DateTime fromDate, DateTime toDate);
    Task<byte[]> ExportMonthlyPayrollAsync(int companyId, int payrollSheetId);
    Task<byte[]> ExportDailySalarySheetAsync(int companyId, DateTime date);
    Task<byte[]> ExportDailySalarySummaryAsync(int companyId, DateTime fromDate, DateTime toDate);
    Task<byte[]> ExportEidBonusAsync(int companyId, int year, int bonusType);
    Task<byte[]> ExportManpowerAsync(int companyId, DateTime date);
    Task<byte[]> ExportDailyOvertimeAsync(int companyId, DateTime fromDate, DateTime toDate);
    Task<byte[]> ExportMonthlyReportAsync(int companyId, int year, int month);
    Task<byte[]> ExportPayrollSummaryAsync(int companyId, int sheetId);
    Task<byte[]> ExportAbsentStatusAsync(int companyId, DateTime date);
    Task<byte[]> ExportOtDeductionAsync(int companyId, int periodId);
    Task<byte[]> ExportHolidaysAsync(int companyId, int year);
    Task<byte[]> ExportMonthlyLeaveRecordAsync(int companyId, int year, int month);
    Task<byte[]> ExportEarnLeaveAsync(int companyId, int year);
    Task<byte[]> ExportVacancyReportAsync(int companyId, int year, int month);
    Task<byte[]> ExportHiringRequestsAsync(int companyId, HiringRequestStatus? status);
    Task<byte[]> ExportSeparationsAsync(int companyId, DateTime? from, DateTime? to, SeparationType? separationType, int? departmentId);
}
