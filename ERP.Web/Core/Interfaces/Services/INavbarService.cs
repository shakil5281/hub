namespace ERP.Web.Core.Interfaces.Services;

public interface INavbarService
{
    Task<GlobalSearchResponseVm> SearchAsync(string? query);
    Task<IReadOnlyList<GlobalSearchItemVm>> GetQuickLinksAsync();
    Task<NotificationsResponseVm> GetNotificationsAsync();
}

public class GlobalSearchResponseVm
{
    public IReadOnlyList<GlobalSearchGroupVm> Groups { get; init; } = Array.Empty<GlobalSearchGroupVm>();
}

public class GlobalSearchGroupVm
{
    public string Category { get; init; } = string.Empty;
    public IReadOnlyList<GlobalSearchItemVm> Items { get; init; } = Array.Empty<GlobalSearchItemVm>();
}

public class GlobalSearchItemVm
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string Icon { get; init; } = "file";
}

public class NotificationsResponseVm
{
    public int UnreadCount { get; init; }
    public IReadOnlyList<NotificationItemVm> Items { get; init; } = Array.Empty<NotificationItemVm>();
}

public class NotificationItemVm
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
