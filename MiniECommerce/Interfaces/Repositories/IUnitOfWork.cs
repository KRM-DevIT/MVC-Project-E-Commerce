namespace MiniECommerce.Interfaces.Repositories
{
    using Microsoft.EntityFrameworkCore.Storage;


    public interface IUnitOfWork : IDisposable
    {
        // Transaction management
        Task<int> SaveChangesAsync();
        int SaveChanges();
        Task<IDbContextTransaction> BeginTransactionAsync();
        IDbContextTransaction BeginTransaction();

    }
}
