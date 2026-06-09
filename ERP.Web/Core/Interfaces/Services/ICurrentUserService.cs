namespace ERP.Web.Core.Interfaces.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    int CompanyId { get; }
    bool IsAuthenticated { get; }
}
