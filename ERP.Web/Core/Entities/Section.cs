using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Section : BaseEntity
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public Department? Department { get; set; }
    public ICollection<Line> Lines { get; set; } = new List<Line>();
    public ICollection<Designation> Designations { get; set; } = new List<Designation>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
