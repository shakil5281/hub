namespace ERP.Web.Models;

public class SidebarGroupVm
{
    public string Title { get; init; } = string.Empty;
    public bool OpenByDefault { get; set; }
    public IReadOnlyList<SidebarLinkVm> Links { get; init; } = Array.Empty<SidebarLinkVm>();
}

public class SidebarLinkVm
{
    public string Label { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string Icon { get; init; } = "file";
    public bool IsActive { get; set; }
}
