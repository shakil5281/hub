using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.GrossSalary).HasPrecision(18, 2);
        builder.Property(e => e.BasicSalary).HasPrecision(18, 2);
        builder.Property(e => e.HouseRentAllowance).HasPrecision(18, 2);
        builder.Property(e => e.MedicalAllowance).HasPrecision(18, 2);
        builder.Property(e => e.TransportAllowance).HasPrecision(18, 2);
        builder.Property(e => e.OtherAllowance).HasPrecision(18, 2);
        builder.Property(e => e.SignatureData).HasColumnType("nvarchar(max)");
        builder.Property(e => e.PresentAddress).HasMaxLength(500);
        builder.Property(e => e.PermanentAddress).HasMaxLength(500);
        builder.Property(e => e.Email).HasMaxLength(150);
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeCode }).IsUnique();
        builder.HasIndex(e => new { e.CompanyId, e.PunchNumber }).IsUnique();
        builder.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Section).WithMany(s => s.Employees).HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Line).WithMany(l => l.Employees).HasForeignKey(e => e.LineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Designation).WithMany(d => d.Employees).HasForeignKey(e => e.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Address).WithMany().HasForeignKey(e => e.AddressId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}
