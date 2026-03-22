namespace MiniECommerce.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        bool CheckUniquness(String sku);
        List<Product> ProductsByCategory(int categoryId);

        List<Product> ProductsPaginated(int pageNumber, int pageSize);

        List<Product> SearchProducts(string keyword);
    }
}
