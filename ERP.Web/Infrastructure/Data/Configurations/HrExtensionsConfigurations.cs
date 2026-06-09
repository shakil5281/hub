using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class StaffingPlanConfiguration : IEntityTypeConfiguration<StaffingPlan>
{
    public void Configure(EntityTypeBuilder<StaffingPlan> builder)
    {
        builder.Property(s => s.Remarks).HasMaxLength(500);
        builder.HasOne(s => s.Department).WithMany().HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Section).WithMany().HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Line).WithMany().HasForeignKey(s => s.LineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Designation).WithMany().HasForeignKey(s => s.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.CompanyId, s.DepartmentId, s.SectionId, s.LineId, s.DesignationId, s.Year, s.Month }).IsUnique();
    }
}

public class HiringRequestConfiguration : IEntityTypeConfiguration<HiringRequest>
{
    public void Configure(EntityTypeBuilder<HiringRequest> builder)
    {
        builder.Property(h => h.Reason).HasMaxLength(500);
        builder.Property(h => h.ApprovedBy).HasMaxLength(150);
        builder.HasOne(h => h.Department).WithMany().HasForeignKey(h => h.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Section).WithMany().HasForeignKey(h => h.SectionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Line).WithMany().HasForeignKey(h => h.LineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Designation).WithMany().HasForeignKey(h => h.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(h => new { h.CompanyId, h.HiringRequestStatus });
    }
}

public class EmployeeSeparationConfiguration : IEntityTypeConfiguration<EmployeeSeparation>
{
    public void Configure(EntityTypeBuilder<EmployeeSeparation> builder)
    {
        builder.Property(s => s.Reason).HasMaxLength(500);
        builder.Property(s => s.Remarks).HasMaxLength(500);
        builder.HasOne(s => s.Employee).WithMany().HasForeignKey(s => s.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.CompanyId, s.EmployeeId });
        builder.HasIndex(s => new { s.CompanyId, s.SeparationDate });
    }
}
