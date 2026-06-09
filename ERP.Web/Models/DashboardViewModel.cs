namespace ERP.Web.Models;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int OnLeaveToday { get; set; }
    public int PendingLeaveApplications { get; set; }
    public int PendingAttendanceApprovals { get; set; }
    public IList<string> PayrollChartLabels { get; set; } = new List<string>();
    public IList<double> PayrollChartAmounts { get; set; } = new List<double>();
}

public class DashboardQuickLinkVm
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string Icon { get; init; } = "file";
}
