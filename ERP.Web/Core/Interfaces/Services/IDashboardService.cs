using ERP.Web.Models;

namespace ERP.Web.Core.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
}
