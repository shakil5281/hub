using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class PayrollPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PayrollSheetConfiguration : IEntityTypeConfiguration<PayrollSheet>
{
    public void Configure(EntityTypeBuilder<PayrollSheet> builder)
    {
        builder.Property(p => p.TotalNetPayable).HasPrecision(18, 2);
        builder.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.PayrollPeriod).WithMany(p => p.PayrollSheets).HasForeignKey(p => p.PayrollPeriodId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PayrollDetailConfiguration : IEntityTypeConfiguration<PayrollDetail>
{
    public void Configure(EntityTypeBuilder<PayrollDetail> builder)
    {
        builder.Property(p => p.GrossSalary).HasPrecision(18, 2);
        builder.Property(p => p.BasicSalary).HasPrecision(18, 2);
        builder.Property(p => p.HouseRent).HasPrecision(18, 2);
        builder.Property(p => p.MedicalAllowance).HasPrecision(18, 2);
        builder.Property(p => p.FoodAllowance).HasPrecision(18, 2);
        builder.Property(p => p.ConveyanceAllowance).HasPrecision(18, 2);
        builder.Property(p => p.AbsentDeduction).HasPrecision(18, 2);
        builder.Property(p => p.OvertimeAmount).HasPrecision(18, 2);
        builder.Property(p => p.NightBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.HolidayBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.WeeklyOffBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.AttendanceBonusAmount).HasPrecision(18, 2);
        builder.Property(p => p.AdvanceDeduction).HasPrecision(18, 2);
        builder.Property(p => p.LoanDeduction).HasPrecision(18, 2);
        builder.Property(p => p.IncrementAdjustment).HasPrecision(18, 2);
        builder.Property(p => p.NetPayableSalary).HasPrecision(18, 2);
        builder.HasOne(p => p.PayrollSheet).WithMany(s => s.PayrollDetails).HasForeignKey(p => p.PayrollSheetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Employee).WithMany(e => e.PayrollDetails).HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.SalaryStructure).WithMany(s => s.PayrollDetails).HasForeignKey(p => p.SalaryStructureId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.MonthlyAttendanceSummary).WithMany(s => s.PayrollDetails).HasForeignKey(p => p.MonthlyAttendanceSummaryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.Property(l => l.LeaveCode).HasMaxLength(40);
        builder.HasOne(l => l.Company).WithMany().HasForeignKey(l => l.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.LeaveCode }).IsUnique().HasFilter("[LeaveCode] IS NOT NULL");
    }
}

public class LeaveApplicationConfiguration : IEntityTypeConfiguration<LeaveApplication>
{
    public void Configure(EntityTypeBuilder<LeaveApplication> builder)
    {
        builder.HasOne(l => l.Employee).WithMany(e => e.LeaveApplications).HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.LeaveType).WithMany(t => t.LeaveApplications).HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.Property(s => s.ShiftCode).HasMaxLength(40);
        builder.HasOne(s => s.Company).WithMany().HasForeignKey(s => s.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.CompanyId, s.ShiftCode }).IsUnique().HasFilter("[ShiftCode] IS NOT NULL");
    }
}

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.HasOne(s => s.Employee).WithMany(e => e.ShiftAssignments).HasForeignKey(s => s.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Shift).WithMany(sh => sh.ShiftAssignments).HasForeignKey(s => s.ShiftId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PunchRecordConfiguration : IEntityTypeConfiguration<PunchRecord>
{
    public void Configure(EntityTypeBuilder<PunchRecord> builder)
    {
        builder.Property(p => p.PunchCardNo).HasMaxLength(80);
        builder.Property(p => p.DeviceName).HasMaxLength(150);
        builder.Property(p => p.RawPayload).HasColumnType("nvarchar(max)");
        builder.HasOne(p => p.Employee).WithMany(e => e.PunchRecords).HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.DeviceMaster).WithMany(d => d.PunchRecords).HasForeignKey(p => p.DeviceMasterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.EmployeeDeviceMapping).WithMany(d => d.PunchRecords).HasForeignKey(p => p.EmployeeDeviceMappingId).OnDelete(DeleteBehavior.Restrict);
    }
}
