using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class PayrollSheet : BaseEntity
{
    public int PayrollPeriodId { get; set; }
    public PayrollStatus PayrollStatus { get; set; } = PayrollStatus.Draft;
    public decimal TotalNetPayable { get; set; }

    public PayrollPeriod? PayrollPeriod { get; set; }
    public Company? Company { get; set; }
    public ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
}
