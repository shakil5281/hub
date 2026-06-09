using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class BillRateConfig : BaseEntity
{
    public BillType BillType { get; set; }
    public BillRateType RateType { get; set; }
    public decimal Amount { get; set; }
    public int? ShiftId { get; set; }

    public Company? Company { get; set; }
    public Shift? Shift { get; set; }
}
