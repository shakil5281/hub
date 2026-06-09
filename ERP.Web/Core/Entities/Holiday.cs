using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class Holiday : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; }
    public HolidayType HolidayType { get; set; } = HolidayType.Public;
    public string? Description { get; set; }

    public Company? Company { get; set; }
}
