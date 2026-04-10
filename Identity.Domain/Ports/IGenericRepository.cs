using System.Linq.Expressions;
using Identity.Domain.Common.Wrappers;
using Identity.Domain.Entities.Base;

namespace Identity.Domain.Ports;

public interface IGenericRepository<E> : IDisposable
       where E : BaseEntity<int>
{
    Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter = null,
                  Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null, string includeStringProperties = "",
                  bool isTracking = false);

    Task<List<IGrouping<string, E>>> GetGroupedAsync(Expression<Func<E, string>> keySelector);
    Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
         bool isTracking = false, params Expression<Func<E, object>>[] includeObjectProperties);

    Task<PaginatedResponse<E>> GetPaginatedAsync(Expression<Func<E, bool>>? filter = null,
            Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
            int? pageNumber = null, int? pageSize = null,
            bool isTracking = false, params Expression<Func<E, object>>[] includeObjectProperties);

    Task<PaginatedResponse<E>> GetPaginatedAsync(Expression<Func<E, bool>>? filter = null,
            Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
            int? pageNumber = null, int? pageSize = null,
            bool isTracking = false, string includeStringProperties = "");

    IAsyncEnumerable<E> GetAsyncStream(Expression<Func<E, bool>>? filter = null, Func<IQueryable<E>,
        IOrderedQueryable<E>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false);

    Task<E?> GetByIdAsync(object id);
    Task<E?> GetByIdAsync(object id, string includeStringProperties = "", bool isTracking = false);

    Task<E> AddAsync(E entity, params Expression<Func<E, object?>>[] includes);
    Task<IEnumerable<E>> AddAsync(IEnumerable<E> entities);
    Task UpdateAsync(E entity);
    Task DeleteAsync(E entity);
    Task DeleteAsync(IEnumerable<E> entities);
    Task<bool> Exist(Expression<Func<E, bool>> filter);
    Task<E?> GetEntityAsync(Expression<Func<E, bool>> filter, bool isTracking = false);
    string GetUnaccent(string field);
}
