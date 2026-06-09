using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class PayrollPeriod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PayrollStatus PayrollStatus { get; set; } = PayrollStatus.Draft;

    public Company? Company { get; set; }
    public ICollection<PayrollSheet> PayrollSheets { get; set; } = new List<PayrollSheet>();
}
