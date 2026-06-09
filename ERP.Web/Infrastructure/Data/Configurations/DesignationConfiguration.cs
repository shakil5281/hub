using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
{
    public void Configure(EntityTypeBuilder<Designation> builder)
    {
        builder.HasIndex(d => new { d.CompanyId, d.Code }).IsUnique();
        builder.HasOne(d => d.Company).WithMany().HasForeignKey(d => d.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Section).WithMany(s => s.Designations).HasForeignKey(d => d.SectionId).OnDelete(DeleteBehavior.Restrict);
    }
}
