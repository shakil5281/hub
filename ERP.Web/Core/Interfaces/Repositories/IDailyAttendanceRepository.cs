using ERP.Web.Core.Entities;

namespace ERP.Web.Core.Interfaces.Repositories;

public interface IDailyAttendanceRepository : IRepository<DailyAttendance>
{
    Task<int> CountPresentTodayAsync(int companyId, DateTime date);
    Task<int> CountAbsentTodayAsync(int companyId, DateTime date);
}
