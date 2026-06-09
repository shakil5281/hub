namespace ERP.Web.Models;

public class PageHeaderVm
{
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public (string Text, string Href, string Variant)[] Actions { get; init; } = Array.Empty<(string, string, string)>();
}
