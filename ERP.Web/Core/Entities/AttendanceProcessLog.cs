using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class AttendanceProcessLog : BaseEntity
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int ProcessedRows { get; set; }
    public int SkippedRows { get; set; }
}
