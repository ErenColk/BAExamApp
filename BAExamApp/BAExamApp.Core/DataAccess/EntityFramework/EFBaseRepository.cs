using BAExamApp.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace BAExamApp.Core.DataAccess.EntityFramework;

public abstract class EFBaseRepository<TEntity> : IAsyncOrderableRepository<TEntity>, IAsyncFindableRepository<TEntity>, IAsyncQueryableRepository<TEntity>, IAsyncInsertableRepository<TEntity>, IAsyncUpdateableRepository<TEntity>, IAsyncDeleteableRepository<TEntity>, IAsyncRepository, IAsyncTransactionRepository, IRepository
    where TEntity : BaseEntity
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _table;

    public EFBaseRepository(DbContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var entry = await _table.AddAsync(entity);

        return entry.Entity;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return _table.AddRangeAsync(entities);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        return expression is null ? GetAllActives().AnyAsync() : GetAllActives().AnyAsync(expression);
    }

    public void Delete(TEntity entity)
    {
        _table.Remove(entity);
    }

    public Task DeleteAsync(TEntity entity)
    {
        return Task.FromResult(_table.Remove(entity));
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true)
    {
        return await GetAllActives(tracking).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true)
    {
        return await GetAllActives(tracking).Where(expression).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true)
    {
        var values = GetAllActives(tracking);

        return orderDesc ? await values.OrderByDescending(orderby).ToListAsync() : await values.OrderBy(orderby).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true)
    {
        var values = GetAllActives(tracking).Where(expression);

        return orderDesc ? await values.OrderByDescending(orderby).ToListAsync() : await values.OrderBy(orderby).ToListAsync();
    }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true)
    {
        return GetAllActives(tracking).FirstOrDefaultAsync(expression);
    }

    public Task<TEntity?> GetByIdAsync(Guid id, bool tracking = true)
    {
        var values = GetAllActives(tracking);

        return values.FirstOrDefaultAsync(x => x.Id == id);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var entry = await Task.FromResult(_table.Update(entity));
        return entry.Entity;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task<IExecutionStrategy> CreateExecutionStrategy()
    {
        return Task.FromResult(_context.Database.CreateExecutionStrategy());
    }




    protected IQueryable<TEntity> GetAllActives(bool tracking = true)
    {
        var values = _table.Where(x => x.Status != Status.Deleted);

        return tracking ? values : values.AsNoTracking();
    }

    public Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _table.RemoveRange(entities);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllDataAsync(bool tracking = true)
    {
        return tracking ? await _table.ToListAsync() : await _table.AsNoTracking().ToListAsync();
    }
}
