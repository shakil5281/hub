using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class DailyAttendanceConfiguration : IEntityTypeConfiguration<DailyAttendance>
{
    public void Configure(EntityTypeBuilder<DailyAttendance> builder)
    {
        builder.HasIndex(a => new { a.CompanyId, a.EmployeeId, a.AttendanceDate }).IsUnique();
        builder.Property(a => a.PayableDayValue).HasPrecision(18, 2);
        builder.HasOne(a => a.Employee).WithMany(e => e.DailyAttendances).HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Shift).WithMany().HasForeignKey(a => a.ShiftId).OnDelete(DeleteBehavior.Restrict);
    }
}
