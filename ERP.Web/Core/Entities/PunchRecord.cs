using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class PunchRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime PunchTime { get; set; }
    public PunchType PunchType { get; set; }
    public PunchSource Source { get; set; } = PunchSource.Device;
    public string? DeviceId { get; set; }

    public Employee? Employee { get; set; }
}
