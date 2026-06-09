using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Line : BaseEntity
{
    public int SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public Section? Section { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
