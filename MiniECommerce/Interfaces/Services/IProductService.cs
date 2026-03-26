namespace MiniECommerce.Interfaces.Services
{
    public interface IProductService
    {
        // Get All Products
        List<Product> GetAllProducts();

        // Get Product By Id
        Product? GetProductById(int id);

        // Create Product
        bool CreateProduct(Product Product);

        // Update Product
        bool UpdateProduct(Product Product);

        // Delete Product
        bool DeleteProduct(int id);

        // Search Products
        List<Product> SearchProductsByKeyword(string keyword);

        // Get Products By Category
        List<Product> GetProductsByCategory(int categoryId);

        // Pagination
        List<Product> GetProductsWithPagination(int pageNumber, int pageSize);

        List<Product> GetProductsByIDs(List<int> productIds);

        int GetProductCount();
    }
}
