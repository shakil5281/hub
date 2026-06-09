using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class OrganogramConfigurations :
    IEntityTypeConfiguration<Section>,
    IEntityTypeConfiguration<Line>,
    IEntityTypeConfiguration<CompanyAddress>,
    IEntityTypeConfiguration<LeaveBalance>,
    IEntityTypeConfiguration<BillRateConfig>,
    IEntityTypeConfiguration<AdvanceSalary>,
    IEntityTypeConfiguration<SalaryIncrement>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasIndex(s => new { s.DepartmentId, s.Code }).IsUnique();
        builder.HasOne(s => s.Department).WithMany(d => d.Sections).HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<Line> builder)
    {
        builder.HasIndex(l => new { l.SectionId, l.Code }).IsUnique();
        builder.HasOne(l => l.Section).WithMany(s => s.Lines).HasForeignKey(l => l.SectionId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<CompanyAddress> builder)
    {
        builder.HasOne(a => a.Company).WithMany(c => c.Addresses).HasForeignKey(a => a.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.HasIndex(l => new { l.CompanyId, l.EmployeeId, l.LeaveTypeId, l.Year }).IsUnique();
        builder.Property(l => l.OpeningBalance).HasPrecision(18, 2);
        builder.Property(l => l.EarnedDays).HasPrecision(18, 2);
        builder.Property(l => l.AdjustedDays).HasPrecision(18, 2);
        builder.HasOne(l => l.Employee).WithMany(e => e.LeaveBalances).HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<BillRateConfig> builder)
    {
        builder.Property(b => b.Amount).HasPrecision(18, 2);
        builder.HasOne(b => b.Company).WithMany().HasForeignKey(b => b.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Shift).WithMany().HasForeignKey(b => b.ShiftId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<AdvanceSalary> builder)
    {
        builder.Property(a => a.Amount).HasPrecision(18, 2);
        builder.Property(a => a.MonthlyDeduction).HasPrecision(18, 2);
        builder.Property(a => a.RemainingBalance).HasPrecision(18, 2);
        builder.HasOne(a => a.Employee).WithMany(e => e.AdvanceSalaries).HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<SalaryIncrement> builder)
    {
        builder.Property(s => s.PreviousGross).HasPrecision(18, 2);
        builder.Property(s => s.NewGross).HasPrecision(18, 2);
        builder.HasOne(s => s.Employee).WithMany(e => e.SalaryIncrements).HasForeignKey(s => s.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
