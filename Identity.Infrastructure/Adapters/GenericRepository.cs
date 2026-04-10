
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Common.Exceptions;
using Identity.Domain.Common.Wrappers;
using Identity.Domain.Entities.Base;
using Identity.Domain.Ports;
using Identity.Infrastructure.Contexts;
using System.Data;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Adapters;

public class GenericRepository<E> : IGenericRepository<E> where E : BaseEntity<int>
{
    private readonly PersistenceContext _context;
    public GenericRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<E> AddAsync(E entity, params Expression<Func<E, object?>>[] includes)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");

        await _context.Set<E>().AddAsync(entity);
        await CommitAsync();

        foreach (var include in includes)
        {
            await _context.Entry(entity).Reference(include).LoadAsync();
        }

        return entity;
    }

    public async Task<IEnumerable<E>> AddAsync(IEnumerable<E> entities)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities), "Entity can not be null");
        _context.Set<E>().AddRange(entities);
        await CommitAsync();
        return entities;
    }


    public async Task DeleteAsync(IEnumerable<E> entities)
    {
        if (entities != null)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
                entity.SetDelete();
            }
            _context.Set<E>().UpdateRange(entities);
            await CommitAsync();
        }
    }
    public async Task DeleteAsync(E entity)
    {
        if (entity != null)
        {
            entity.MarkAsDeleted();
            entity.SetDelete();
            _context.Set<E>().Update(entity);
            await CommitAsync();
        }
    }

    public async Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter = null, Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null, string includeStringProperties = "", bool isTracking = false)
    {
        IQueryable<E> query = _context.Set<E>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync().ConfigureAwait(false);
        }

        return (!isTracking) ? await query.AsNoTracking().ToListAsync() : await query.ToListAsync();
    }

    public async Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter = null, Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null, bool isTracking = false, params Expression<Func<E, object>>[] includeObjectProperties)
    {
        IQueryable<E> query = _context.Set<E>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeObjectProperties != null)
        {
            foreach (Expression<Func<E, object>> include in includeObjectProperties)
            {
                query = query.Include(include);
            }
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return (!isTracking) ? await query.AsNoTracking().ToListAsync() : await query.ToListAsync();
    }

    public async Task<E?> GetByIdAsync(object id)
    {
        return await _context.Set<E>().FindAsync(id);
    }

    public async Task<E?> GetByIdAsync(object id, string includeStringProperties = "", bool isTracking = false)
    {
        IQueryable<E> query = _context.Set<E>();

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == (int)id);
    }


    public async Task<bool> Exist(Expression<Func<E, bool>> filter)
    {
        return await _context.Set<E>().AnyAsync(filter);
    }

        
    public async Task UpdateAsync(E entity)
    {
        if (entity != null)
        {

            _context.Set<E>().Update(entity);
            await CommitAsync();
        }
    }

    private async Task CommitAsync()
    {
        
            _context.ChangeTracker.DetectChanges();

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    default:
                        throw new CustomException($"Error al actualizar la entidad {nameof(entry)}");
                }
            }
        try
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            Console.WriteLine(ex.InnerException?.StackTrace);
        }
        await _context.CommitAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        this._context.Dispose();
    }

    public async Task<PaginatedResponse<E>> GetPaginatedAsync(Expression<Func<E, bool>>? filter = null,
    Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
   int? pageNumber = null, int? pageSize = null,
    bool isTracking = false, params Expression<Func<E, object>>[] includeObjectProperties)
    {
        IQueryable<E> query = _context.Set<E>();
        var totalcountrecords = await query.CountAsync();
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeObjectProperties)
        {
            query = query.Include(includeProperty);
        }


        if (!isTracking)
        {
            query = query.AsNoTracking();
        }

        var totalRecords = await query.CountAsync();

        List<E> data;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            data = await query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();
        }
        else
        {
            data = await query.ToListAsync();
        }

        return new PaginatedResponse<E>(
            data,
            pageNumber ?? 1,
            pageSize ?? totalRecords,
            totalRecords,
            totalcountrecords);
    }

    public async Task<PaginatedResponse<E>> GetPaginatedAsync(Expression<Func<E, bool>>? filter = null, Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null, int? pageNumber = null, int? pageSize = null, bool isTracking = false, string includeStringProperties = "")
    {
        IQueryable<E> query = _context.Set<E>();
        var totalcountrecords = await query.CountAsync();
        
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }


        if (!isTracking)
        {
            query = query.AsNoTracking();
        }

        var totalRecords = await query.CountAsync();

        List<E> data;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            data = await query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();
        }
        else
        {
            data = await query.ToListAsync();
        }

        return new PaginatedResponse<E>(
            data,
            pageNumber ?? 1,
            pageSize ?? totalRecords,
            totalRecords,
            totalcountrecords);
    }

    public async Task<List<IGrouping<string, E>>> GetGroupedAsync(Expression<Func<E, string>> keySelector)
    {
        var query = _context.Set<E>();
        var groupedData = await query.GroupBy(keySelector).ToListAsync();
        return groupedData;
    }

   
    public async Task<E?> GetEntityAsync(Expression<Func<E, bool>> filter, bool isTracking = false)
    {
        
        return (!isTracking) ? await _context.Set<E>().AsNoTracking().FirstOrDefaultAsync(filter) : await _context.Set<E>().FirstOrDefaultAsync(filter);
    }


    public IAsyncEnumerable<E> GetAsyncStream(
     Expression<Func<E, bool>>? filter = null,
     Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
     string includeStringProperties = "",
     bool isTracking = false)
    {
        IQueryable<E> query = _context.Set<E>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrWhiteSpace(includeStringProperties))
        {
            foreach (var includeProperty in includeStringProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (!isTracking)
        {
            query = query.AsNoTracking();
        }

        return query.AsAsyncEnumerable();
    }

    public string GetUnaccent(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            throw new CustomException("Campo no puede ser null o vacio.");
        }
        return PersistenceContext.Unaccent(field);
    }
}
