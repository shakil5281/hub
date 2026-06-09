using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class ShiftAssignment : BaseEntity
{
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public Employee? Employee { get; set; }
    public Shift? Shift { get; set; }
}
