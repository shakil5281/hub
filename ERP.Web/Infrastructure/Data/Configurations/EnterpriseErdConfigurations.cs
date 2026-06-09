using ERP.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Web.Infrastructure.Data.Configurations;

public class EmployeeAddressConfiguration : IEntityTypeConfiguration<EmployeeAddress>
{
    public void Configure(EntityTypeBuilder<EmployeeAddress> builder)
    {
        builder.Property(e => e.AddressLine).HasMaxLength(500);
        builder.HasOne(e => e.Employee).WithMany(e => e.EmployeeAddresses).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.AddressType });
    }
}

public class EmployeeJobHistoryConfiguration : IEntityTypeConfiguration<EmployeeJobHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeJobHistory> builder)
    {
        builder.Property(e => e.Reason).HasMaxLength(500);
        builder.HasOne(e => e.Employee).WithMany(e => e.JobHistories).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Department).WithMany().HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Section).WithMany().HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Line).WithMany().HasForeignKey(e => e.LineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Designation).WithMany().HasForeignKey(e => e.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.EffectiveFrom });
    }
}

public class EmployeeSalaryStructureConfiguration : IEntityTypeConfiguration<EmployeeSalaryStructure>
{
    public void Configure(EntityTypeBuilder<EmployeeSalaryStructure> builder)
    {
        builder.Property(e => e.GrossSalary).HasPrecision(18, 2);
        builder.Property(e => e.BasicSalary).HasPrecision(18, 2);
        builder.Property(e => e.HouseRent).HasPrecision(18, 2);
        builder.Property(e => e.MedicalAllowance).HasPrecision(18, 2);
        builder.Property(e => e.FoodAllowance).HasPrecision(18, 2);
        builder.Property(e => e.ConveyanceAllowance).HasPrecision(18, 2);
        builder.Property(e => e.OtherAllowance).HasPrecision(18, 2);
        builder.HasOne(e => e.Employee).WithMany(e => e.SalaryStructures).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.EffectiveFrom });
    }
}

public class EmployeeDeviceMappingConfiguration : IEntityTypeConfiguration<EmployeeDeviceMapping>
{
    public void Configure(EntityTypeBuilder<EmployeeDeviceMapping> builder)
    {
        builder.Property(e => e.PunchCardNo).HasMaxLength(80);
        builder.Property(e => e.DeviceUserId).HasMaxLength(80);
        builder.Property(e => e.DeviceId).HasMaxLength(120);
        builder.HasOne(e => e.Employee).WithMany(e => e.DeviceMappings).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.PunchCardNo });
        builder.HasIndex(e => new { e.CompanyId, e.DeviceId, e.DeviceUserId });
    }
}

public class EmployeeWeeklyOffConfiguration : IEntityTypeConfiguration<EmployeeWeeklyOff>
{
    public void Configure(EntityTypeBuilder<EmployeeWeeklyOff> builder)
    {
        builder.Property(e => e.OffType).HasMaxLength(80);
        builder.HasOne(e => e.Employee).WithMany(e => e.WeeklyOffs).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.DayOfWeek, e.EffectiveFrom });
    }
}

public class DeviceMasterConfiguration : IEntityTypeConfiguration<DeviceMaster>
{
    public void Configure(EntityTypeBuilder<DeviceMaster> builder)
    {
        builder.Property(d => d.DeviceCode).HasMaxLength(80);
        builder.Property(d => d.DeviceName).HasMaxLength(150);
        builder.Property(d => d.IpAddress).HasMaxLength(80);
        builder.HasOne(d => d.Company).WithMany().HasForeignKey(d => d.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(d => new { d.CompanyId, d.DeviceCode }).IsUnique();
    }
}

public class DeviceSyncLogConfiguration : IEntityTypeConfiguration<DeviceSyncLog>
{
    public void Configure(EntityTypeBuilder<DeviceSyncLog> builder)
    {
        builder.Property(d => d.ErrorMessage).HasMaxLength(1000);
        builder.HasOne(d => d.DeviceMaster).WithMany(d => d.SyncLogs).HasForeignKey(d => d.DeviceMasterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(d => new { d.CompanyId, d.DeviceMasterId, d.SyncStartedAt });
    }
}

public class ManualPunchRequestConfiguration : IEntityTypeConfiguration<ManualPunchRequest>
{
    public void Configure(EntityTypeBuilder<ManualPunchRequest> builder)
    {
        builder.Property(m => m.Reason).HasMaxLength(500);
        builder.HasOne(m => m.Employee).WithMany().HasForeignKey(m => m.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.CreatedPunchRecord).WithMany().HasForeignKey(m => m.CreatedPunchRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(m => new { m.CompanyId, m.EmployeeId, m.RequestedPunchTime });
        builder.HasIndex(m => new { m.CompanyId, m.RequestStatus });
    }
}

public class DailyAttendancePunchConfiguration : IEntityTypeConfiguration<DailyAttendancePunch>
{
    public void Configure(EntityTypeBuilder<DailyAttendancePunch> builder)
    {
        builder.HasOne(d => d.DailyAttendance).WithMany().HasForeignKey(d => d.DailyAttendanceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.PunchRecord).WithMany().HasForeignKey(d => d.PunchRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(d => new { d.CompanyId, d.DailyAttendanceId, d.PunchRecordId }).IsUnique();
    }
}

public class AttendanceApprovalLogConfiguration : IEntityTypeConfiguration<AttendanceApprovalLog>
{
    public void Configure(EntityTypeBuilder<AttendanceApprovalLog> builder)
    {
        builder.Property(a => a.Remarks).HasMaxLength(500);
        builder.HasOne(a => a.DailyAttendance).WithMany().HasForeignKey(a => a.DailyAttendanceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(a => new { a.CompanyId, a.DailyAttendanceId, a.ActionAt });
    }
}

public class OvertimeApprovalLogConfiguration : IEntityTypeConfiguration<OvertimeApprovalLog>
{
    public void Configure(EntityTypeBuilder<OvertimeApprovalLog> builder)
    {
        builder.Property(o => o.Remarks).HasMaxLength(500);
        builder.HasOne(o => o.DailyAttendance).WithMany().HasForeignKey(o => o.DailyAttendanceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Employee).WithMany().HasForeignKey(o => o.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(o => new { o.CompanyId, o.EmployeeId, o.ActionAt });
    }
}

public class AttendanceConflictConfiguration : IEntityTypeConfiguration<AttendanceConflict>
{
    public void Configure(EntityTypeBuilder<AttendanceConflict> builder)
    {
        builder.Property(a => a.Description).HasMaxLength(1000);
        builder.HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(a => new { a.CompanyId, a.EmployeeId, a.AttendanceDate, a.ConflictType });
    }
}

public class MonthlyAttendanceSummaryConfiguration : IEntityTypeConfiguration<MonthlyAttendanceSummary>
{
    public void Configure(EntityTypeBuilder<MonthlyAttendanceSummary> builder)
    {
        ConfigureSummaryDecimals(builder);
        builder.HasOne(m => m.Employee).WithMany().HasForeignKey(m => m.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(m => new { m.CompanyId, m.EmployeeId, m.SalaryYear, m.SalaryMonth }).IsUnique();
    }

    private static void ConfigureSummaryDecimals(EntityTypeBuilder<MonthlyAttendanceSummary> builder)
    {
        builder.Property(m => m.PayableDays).HasPrecision(18, 2);
        builder.Property(m => m.PresentDays).HasPrecision(18, 2);
        builder.Property(m => m.AbsentDays).HasPrecision(18, 2);
        builder.Property(m => m.PaidLeaveDays).HasPrecision(18, 2);
        builder.Property(m => m.UnpaidLeaveDays).HasPrecision(18, 2);
        builder.Property(m => m.HolidayDays).HasPrecision(18, 2);
        builder.Property(m => m.HolidayPresentDays).HasPrecision(18, 2);
        builder.Property(m => m.WeeklyOffDays).HasPrecision(18, 2);
        builder.Property(m => m.WeeklyOffPresentDays).HasPrecision(18, 2);
        builder.Property(m => m.LateDays).HasPrecision(18, 2);
        builder.Property(m => m.EarlyOutDays).HasPrecision(18, 2);
        builder.Property(m => m.MissingPunchDays).HasPrecision(18, 2);
    }
}

public class LeavePolicyConfiguration : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.Property(l => l.YearlyQuota).HasPrecision(18, 2);
        builder.Property(l => l.MonthlyAccrual).HasPrecision(18, 2);
        builder.Property(l => l.MaxCarryForwardDays).HasPrecision(18, 2);
        builder.Property(l => l.MaxEncashmentDays).HasPrecision(18, 2);
        builder.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.LeaveTypeId, l.EffectiveFrom });
    }
}

public class LeaveApprovalLogConfiguration : IEntityTypeConfiguration<LeaveApprovalLog>
{
    public void Configure(EntityTypeBuilder<LeaveApprovalLog> builder)
    {
        builder.Property(l => l.Remarks).HasMaxLength(500);
        builder.HasOne(l => l.LeaveApplication).WithMany().HasForeignKey(l => l.LeaveApplicationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.LeaveApplicationId, l.ApprovalLevel });
    }
}

public class LeaveBalanceTransactionConfiguration : IEntityTypeConfiguration<LeaveBalanceTransaction>
{
    public void Configure(EntityTypeBuilder<LeaveBalanceTransaction> builder)
    {
        builder.Property(l => l.Days).HasPrecision(18, 2);
        builder.Property(l => l.Remarks).HasMaxLength(500);
        builder.HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.LeaveApplication).WithMany().HasForeignKey(l => l.LeaveApplicationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.EmployeeId, l.LeaveTypeId, l.TransactionDate });
    }
}

public class LeaveConflictConfiguration : IEntityTypeConfiguration<LeaveConflict>
{
    public void Configure(EntityTypeBuilder<LeaveConflict> builder)
    {
        builder.HasOne(l => l.LeaveApplication).WithMany().HasForeignKey(l => l.LeaveApplicationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.EmployeeId, l.ConflictDate });
    }
}

public class BillRuleConfiguration :
    IEntityTypeConfiguration<NightBillRule>,
    IEntityTypeConfiguration<SpecialDayBillRule>,
    IEntityTypeConfiguration<AttendanceBonusRule>
{
    public void Configure(EntityTypeBuilder<NightBillRule> builder)
    {
        builder.Property(b => b.FixedAmount).HasPrecision(18, 2);
        builder.Property(b => b.HourlyRate).HasPrecision(18, 2);
        builder.HasOne(b => b.Department).WithMany().HasForeignKey(b => b.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Designation).WithMany().HasForeignKey(b => b.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(b => new { b.CompanyId, b.DepartmentId, b.DesignationId, b.EffectiveFrom });
    }

    public void Configure(EntityTypeBuilder<SpecialDayBillRule> builder)
    {
        builder.Property(b => b.FixedAmount).HasPrecision(18, 2);
        builder.Property(b => b.HourlyRate).HasPrecision(18, 2);
        builder.Property(b => b.Multiplier).HasPrecision(18, 2);
        builder.HasOne(b => b.Department).WithMany().HasForeignKey(b => b.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Designation).WithMany().HasForeignKey(b => b.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(b => new { b.CompanyId, b.BillType, b.DepartmentId, b.DesignationId, b.EffectiveFrom });
    }

    public void Configure(EntityTypeBuilder<AttendanceBonusRule> builder)
    {
        builder.Property(b => b.BonusAmount).HasPrecision(18, 2);
        builder.Property(b => b.MaxAbsentAllowed).HasPrecision(18, 2);
        builder.Property(b => b.MaxUnpaidLeaveAllowed).HasPrecision(18, 2);
        builder.Property(b => b.MinimumPresentDays).HasPrecision(18, 2);
        builder.Property(b => b.MinimumPayableDays).HasPrecision(18, 2);
        builder.HasOne(b => b.Department).WithMany().HasForeignKey(b => b.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Designation).WithMany().HasForeignKey(b => b.DesignationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(b => new { b.CompanyId, b.RuleName, b.EffectiveFrom });
    }
}

public class EmployeeBillEntryConfiguration : IEntityTypeConfiguration<EmployeeBillEntry>
{
    public void Configure(EntityTypeBuilder<EmployeeBillEntry> builder)
    {
        builder.Property(e => e.Quantity).HasPrecision(18, 2);
        builder.Property(e => e.Rate).HasPrecision(18, 2);
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.HasOne(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.DailyAttendance).WithMany().HasForeignKey(e => e.DailyAttendanceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.BillYear, e.BillMonth, e.BillType });
    }
}

public class EmployeeBonusEntryConfiguration : IEntityTypeConfiguration<EmployeeBonusEntry>
{
    public void Configure(EntityTypeBuilder<EmployeeBonusEntry> builder)
    {
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.HasOne(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MonthlyAttendanceSummary).WithMany().HasForeignKey(e => e.MonthlyAttendanceSummaryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.AttendanceBonusRule).WithMany().HasForeignKey(e => e.AttendanceBonusRuleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.BonusYear, e.BonusMonth, e.BonusType });
    }
}

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.Property(l => l.LoanAmount).HasPrecision(18, 2);
        builder.Property(l => l.PaidAmount).HasPrecision(18, 2);
        builder.Property(l => l.RemainingAmount).HasPrecision(18, 2);
        builder.HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.EmployeeId, l.LoanStatus });
    }
}

public class LoanInstallmentConfiguration : IEntityTypeConfiguration<LoanInstallment>
{
    public void Configure(EntityTypeBuilder<LoanInstallment> builder)
    {
        builder.Property(l => l.InstallmentAmount).HasPrecision(18, 2);
        builder.HasOne(l => l.Loan).WithMany(l => l.Installments).HasForeignKey(l => l.LoanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(l => new { l.CompanyId, l.LoanId, l.InstallmentYear, l.InstallmentMonth }).IsUnique();
    }
}

public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => new { p.CompanyId, p.SalaryYear, p.SalaryMonth, p.PayrollType });
    }
}

public class PayrollRunDetailConfiguration : IEntityTypeConfiguration<PayrollRunDetail>
{
    public void Configure(EntityTypeBuilder<PayrollRunDetail> builder)
    {
        builder.Property(p => p.GrossSalary).HasPrecision(18, 2);
        builder.Property(p => p.BasicSalary).HasPrecision(18, 2);
        builder.Property(p => p.TotalEarnings).HasPrecision(18, 2);
        builder.Property(p => p.TotalDeductions).HasPrecision(18, 2);
        builder.Property(p => p.OvertimeAmount).HasPrecision(18, 2);
        builder.Property(p => p.NightBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.HolidayBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.WeeklyOffBillAmount).HasPrecision(18, 2);
        builder.Property(p => p.AttendanceBonusAmount).HasPrecision(18, 2);
        builder.Property(p => p.AdvanceDeduction).HasPrecision(18, 2);
        builder.Property(p => p.LoanDeduction).HasPrecision(18, 2);
        builder.Property(p => p.NetPayable).HasPrecision(18, 2);
        builder.HasOne(p => p.PayrollRun).WithMany(r => r.Details).HasForeignKey(p => p.PayrollRunId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.SalaryStructure).WithMany().HasForeignKey(p => p.SalaryStructureId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.MonthlyAttendanceSummary).WithMany().HasForeignKey(p => p.MonthlyAttendanceSummaryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => new { p.CompanyId, p.PayrollRunId, p.EmployeeId }).IsUnique();
    }
}

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.Property(p => p.NetPayable).HasPrecision(18, 2);
        builder.HasOne(p => p.PayrollRunDetail).WithMany(d => d.Payslips).HasForeignKey(p => p.PayrollRunDetailId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => new { p.CompanyId, p.PayslipNo }).IsUnique().HasFilter("[PayslipNo] IS NOT NULL");
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(a => a.OldValue).HasColumnType("nvarchar(max)");
        builder.Property(a => a.NewValue).HasColumnType("nvarchar(max)");
        builder.HasIndex(a => new { a.CompanyId, a.TableName, a.RecordId, a.ActionAt });
    }
}

public class ServiceErrorLogConfiguration : IEntityTypeConfiguration<ServiceErrorLog>
{
    public void Configure(EntityTypeBuilder<ServiceErrorLog> builder)
    {
        builder.Property(s => s.ErrorMessage).HasColumnType("nvarchar(max)");
        builder.Property(s => s.RequestPayload).HasColumnType("nvarchar(max)");
        builder.HasIndex(s => new { s.CompanyId, s.ServiceName, s.CreatedAt });
    }
}
