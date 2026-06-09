using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class EmployeeSeparation : BaseEntity
{
    public int EmployeeId { get; set; }
    public SeparationType SeparationType { get; set; }
    public DateTime SeparationDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    public string? Reason { get; set; }
    public string? Remarks { get; set; }

    public Employee? Employee { get; set; }
}
