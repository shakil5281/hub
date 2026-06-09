using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class EidBonus : BaseEntity
{
    public int EmployeeId { get; set; }
    public BonusType BonusType { get; set; } = BonusType.EidUlFitr;
    public int Year { get; set; }
    public decimal BonusAmount { get; set; }
    public BonusStatus BonusStatus { get; set; } = BonusStatus.Draft;
    public string? Remarks { get; set; }

    public Employee? Employee { get; set; }
}
