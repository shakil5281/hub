using ERP.Web.Core.Entities;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class DailyAttendanceRepository : Repository<DailyAttendance>, IDailyAttendanceRepository
{
    public DailyAttendanceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountPresentTodayAsync(int companyId, DateTime date)
    {
        return await DbSet.CountAsync(a =>
            a.CompanyId == companyId &&
            a.AttendanceDate.Date == date.Date &&
            !a.IsAbsent &&
            a.InTime.HasValue);
    }

    public async Task<int> CountAbsentTodayAsync(int companyId, DateTime date)
    {
        return await DbSet.CountAsync(a =>
            a.CompanyId == companyId &&
            a.AttendanceDate.Date == date.Date &&
            a.IsAbsent);
    }
}
