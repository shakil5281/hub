namespace ERP.Web.Infrastructure.Data;

public static partial class DbSeeder
{
    public static readonly string[] PermissionCodes =
    [
        "HR.Employee.View", "HR.Employee.Create", "HR.Employee.Edit", "HR.Employee.Delete",
        "HR.Manpower.View", "HR.Manpower.Manage",
        "HR.HiringRequest.View", "HR.HiringRequest.Manage", "HR.HiringRequest.Approve",
        "HR.Separation.View", "HR.Separation.Manage",
        "Admin.User.View", "Admin.User.Create", "Admin.User.Edit", "Admin.User.Delete",
        "Admin.Role.Manage",
        "Company.View", "Company.Edit", "Organogram.Manage",
        "Shift.Manage",
        "Attendance.Process", "Attendance.Punch.Import", "Attendance.Report.View",
        "Leave.Approve", "Leave.Manage",
        "Payroll.View", "Payroll.Generate", "Payroll.Advance.Manage", "Payroll.Increment.Manage", "Payroll.Bill.Config",
        "Shift.Temporary.Manage",
        "Payroll.DailySalary.View",
        "Payroll.EidBonus.Manage",
        "Leave.Holiday.Manage",
        "Leave.MonthlyRecord.View",
        "Leave.EarnLeave.View",
        "Attendance.Manpower.View",
        "Attendance.JobCard.Manage",
        "Attendance.AbsentStatus.View",
        "Attendance.OT.View",
        "Report.Monthly.View"
    ];
}
