using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
{
    public void Configure(EntityTypeBuilder<SavedFilter> builder)
    {
        builder.HasIndex(f => new { f.CompanyId, f.UserId, f.Module, f.Name }).IsUnique();
        builder.HasIndex(f => new { f.CompanyId, f.Module });
    }
}

public class PerformanceIndexConfiguration :
    IEntityTypeConfiguration<Employee>,
    IEntityTypeConfiguration<DailyAttendance>,
    IEntityTypeConfiguration<PunchRecord>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(e => new { e.CompanyId, e.DepartmentId });
        builder.HasIndex(e => new { e.CompanyId, e.SectionId });
        builder.HasIndex(e => new { e.CompanyId, e.Status });
        builder.HasIndex(e => new { e.CompanyId, e.FullName });
    }

    public void Configure(EntityTypeBuilder<DailyAttendance> builder)
    {
        builder.HasIndex(a => new { a.CompanyId, a.AttendanceDate });
        builder.HasIndex(a => new { a.CompanyId, a.IsAbsent, a.AttendanceDate });
    }

    public void Configure(EntityTypeBuilder<PunchRecord> builder)
    {
        builder.HasIndex(p => new { p.CompanyId, p.PunchTime });
        builder.HasIndex(p => new { p.CompanyId, p.EmployeeId, p.PunchTime });
    }
}
