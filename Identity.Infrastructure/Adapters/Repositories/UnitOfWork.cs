using Identity.Domain.Ports;
using Identity.Infrastructure.Contexts;

namespace Identity.Infrastructure.Adapters.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PersistenceContext _context;

    public UnitOfWork(PersistenceContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}