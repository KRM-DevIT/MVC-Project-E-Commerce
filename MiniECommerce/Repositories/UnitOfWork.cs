using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        int result = await _context.SaveChangesAsync();

        return result;
    }

    public int SaveChanges()
    {
        int result = _context.SaveChanges();

        return result;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        IDbContextTransaction transaction =
            await _context.Database.BeginTransactionAsync();

        return transaction;
    }

    public IDbContextTransaction BeginTransaction()
    {
        IDbContextTransaction transaction =
            _context.Database.BeginTransaction();

        return transaction;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}