using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class SavedFilter : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FilterJson { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
