using ERP.Web.Core.Common;
using ERP.Web.Core.Enums;

namespace ERP.Web.Core.Entities;

public class PunchRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? DeviceMasterId { get; set; }
    public int? EmployeeDeviceMappingId { get; set; }
    public string? PunchCardNo { get; set; }
    public DateTime PunchTime { get; set; }
    public PunchType PunchType { get; set; }
    public PunchSource Source { get; set; } = PunchSource.Device;
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? VerifyType { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsProcessed { get; set; }
    public string? RawPayload { get; set; }

    public Employee? Employee { get; set; }
    public DeviceMaster? DeviceMaster { get; set; }
    public EmployeeDeviceMapping? EmployeeDeviceMapping { get; set; }
}
