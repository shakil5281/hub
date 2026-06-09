using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class TemporaryShiftAssignmentConfiguration : IEntityTypeConfiguration<TemporaryShiftAssignment>
{
    public void Configure(EntityTypeBuilder<TemporaryShiftAssignment> builder)
    {
        builder.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Shift).WithMany().HasForeignKey(t => t.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => new { t.EmployeeId, t.AssignmentDate });
    }
}

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.HasOne(h => h.Company).WithMany().HasForeignKey(h => h.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(h => h.Name).HasMaxLength(200);
        builder.HasIndex(h => new { h.CompanyId, h.HolidayDate });
    }
}

public class EidBonusConfiguration : IEntityTypeConfiguration<EidBonus>
{
    public void Configure(EntityTypeBuilder<EidBonus> builder)
    {
        builder.Property(e => e.BonusAmount).HasPrecision(18, 2);
        builder.HasOne(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.Year, e.BonusType });
    }
}

public class JobCardConfiguration : IEntityTypeConfiguration<JobCard>
{
    public void Configure(EntityTypeBuilder<JobCard> builder)
    {
        builder.Property(j => j.TargetQty).HasPrecision(18, 2);
        builder.Property(j => j.AchievedQty).HasPrecision(18, 2);
        builder.HasOne(j => j.Employee).WithMany().HasForeignKey(j => j.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(j => j.Line).WithMany().HasForeignKey(j => j.LineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(j => new { j.CompanyId, j.WorkDate });
    }
}
