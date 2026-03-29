namespace MiniECommerce.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        bool CheckUniquness(String sku);
        List<Product> ProductsByCategory(int categoryId);

        List<Product> ProductsPaginated(int pageNumber, int pageSize);

        List<Product> SearchProducts(string keyword);

        List<Product> FilterProductsByIds(List<int> productIds);

        int GetProductsTotalCount();

        int GetProductsByCategoryCount(List<int> categoryIds);

        List<Product> GetProductsByCategoryWithPagination(List<int> categoryIds, int pageNumber, int pageSize);
    }
}
