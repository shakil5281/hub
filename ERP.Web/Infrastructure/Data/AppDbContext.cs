using ERP.Web.Core.Entities;
using ERP.Web.Core.Entities.Security;
using ERP.Web.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyAddress> CompanyAddresses => Set<CompanyAddress>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Line> Lines => Set<Line>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<PunchRecord> PunchRecords => Set<PunchRecord>();
    public DbSet<DailyAttendance> DailyAttendances => Set<DailyAttendance>();
    public DbSet<AttendanceProcessLog> AttendanceProcessLogs => Set<AttendanceProcessLog>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollSheet> PayrollSheets => Set<PayrollSheet>();
    public DbSet<PayrollDetail> PayrollDetails => Set<PayrollDetail>();
    public DbSet<BillRateConfig> BillRateConfigs => Set<BillRateConfig>();
    public DbSet<AdvanceSalary> AdvanceSalaries => Set<AdvanceSalary>();
    public DbSet<SalaryIncrement> SalaryIncrements => Set<SalaryIncrement>();
    public DbSet<SavedFilter> SavedFilters => Set<SavedFilter>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TemporaryShiftAssignment> TemporaryShiftAssignments => Set<TemporaryShiftAssignment>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<EidBonus> EidBonuses => Set<EidBonus>();
    public DbSet<JobCard> JobCards => Set<JobCard>();
    public DbSet<StaffingPlan> StaffingPlans => Set<StaffingPlan>();
    public DbSet<HiringRequest> HiringRequests => Set<HiringRequest>();
    public DbSet<EmployeeSeparation> EmployeeSeparations => Set<EmployeeSeparation>();
    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();
    public DbSet<EmployeeJobHistory> EmployeeJobHistories => Set<EmployeeJobHistory>();
    public DbSet<EmployeeSalaryStructure> EmployeeSalaryStructures => Set<EmployeeSalaryStructure>();
    public DbSet<EmployeeDeviceMapping> EmployeeDeviceMappings => Set<EmployeeDeviceMapping>();
    public DbSet<EmployeeWeeklyOff> EmployeeWeeklyOffs => Set<EmployeeWeeklyOff>();
    public DbSet<DeviceMaster> DeviceMasters => Set<DeviceMaster>();
    public DbSet<DeviceSyncLog> DeviceSyncLogs => Set<DeviceSyncLog>();
    public DbSet<ManualPunchRequest> ManualPunchRequests => Set<ManualPunchRequest>();
    public DbSet<DailyAttendancePunch> DailyAttendancePunches => Set<DailyAttendancePunch>();
    public DbSet<AttendanceApprovalLog> AttendanceApprovalLogs => Set<AttendanceApprovalLog>();
    public DbSet<OvertimeApprovalLog> OvertimeApprovalLogs => Set<OvertimeApprovalLog>();
    public DbSet<AttendanceConflict> AttendanceConflicts => Set<AttendanceConflict>();
    public DbSet<MonthlyAttendanceSummary> MonthlyAttendanceSummaries => Set<MonthlyAttendanceSummary>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();
    public DbSet<LeaveApprovalLog> LeaveApprovalLogs => Set<LeaveApprovalLog>();
    public DbSet<LeaveBalanceTransaction> LeaveBalanceTransactions => Set<LeaveBalanceTransaction>();
    public DbSet<LeaveConflict> LeaveConflicts => Set<LeaveConflict>();
    public DbSet<NightBillRule> NightBillRules => Set<NightBillRule>();
    public DbSet<SpecialDayBillRule> SpecialDayBillRules => Set<SpecialDayBillRule>();
    public DbSet<AttendanceBonusRule> AttendanceBonusRules => Set<AttendanceBonusRule>();
    public DbSet<EmployeeBillEntry> EmployeeBillEntries => Set<EmployeeBillEntry>();
    public DbSet<EmployeeBonusEntry> EmployeeBonusEntries => Set<EmployeeBonusEntry>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanInstallment> LoanInstallments => Set<LoanInstallment>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<PayrollRunDetail> PayrollRunDetails => Set<PayrollRunDetail>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ServiceErrorLog> ServiceErrorLogs => Set<ServiceErrorLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(EmployeeConfiguration).Assembly);
        ApplySoftDeleteFilters(builder);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder builder)
    {
        builder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Section>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Line>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<CompanyAddress>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Designation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Shift>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ShiftAssignment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PunchRecord>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DailyAttendance>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AttendanceProcessLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveApplication>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveBalance>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PayrollPeriod>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PayrollSheet>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PayrollDetail>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<BillRateConfig>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AdvanceSalary>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<SalaryIncrement>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<SavedFilter>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Company>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<TemporaryShiftAssignment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Holiday>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EidBonus>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<JobCard>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffingPlan>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<HiringRequest>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeSeparation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeAddress>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeJobHistory>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeSalaryStructure>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeDeviceMapping>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeWeeklyOff>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DeviceMaster>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DeviceSyncLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ManualPunchRequest>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DailyAttendancePunch>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AttendanceApprovalLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<OvertimeApprovalLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AttendanceConflict>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MonthlyAttendanceSummary>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeavePolicy>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveApprovalLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveBalanceTransaction>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveConflict>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<NightBillRule>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<SpecialDayBillRule>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AttendanceBonusRule>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeBillEntry>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EmployeeBonusEntry>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Loan>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LoanInstallment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PayrollRun>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PayrollRunDetail>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Payslip>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AuditLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ServiceErrorLog>().HasQueryFilter(e => !e.IsDeleted);
    }
}
