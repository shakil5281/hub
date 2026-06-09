namespace ERP.Web.Core.Interfaces.Services;

public interface IExcelImportService
{
    Task<(int Imported, IList<string> Errors)> ImportEmployeesAsync(Stream fileStream, int companyId, string userId);
}
