using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public Company? Company { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
