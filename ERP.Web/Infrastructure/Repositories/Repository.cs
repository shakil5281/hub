using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

    public virtual async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual async Task SoftDeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;

        var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
        if (isDeletedProp != null)
        {
            isDeletedProp.SetValue(entity, true);
            Update(entity);
        }
    }
}
