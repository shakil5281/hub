using ERP.Web.Core.Common;

namespace ERP.Web.Core.Entities;

public class Designation : BaseEntity
{
    public int SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public Section? Section { get; set; }
    public Company? Company { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
