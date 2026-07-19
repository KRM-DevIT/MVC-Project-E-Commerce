using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
        void UpdateProduct(Product Product);

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

        string GenerateUniqueSKU(string productName , string categoryName);

        // Image handling
        Task<(bool Success, string? FilePath, string? Error)> SaveImageAsync(IFormFile file);
        void DeleteImage(string? filePath);

        List<Product> GetProductsByCategoryWithPagination(List<int> categoryIds, int pageNumber, int pageSize);

        int GetProductsByCategoryCount(List<int> categoryIds);
        int GetSearchProductCount(List<int> selectedcategories, string query);
        List<Product> SearchProductsWithPagination(List<int> selectedcategories, string query, int pageNumber, int pageSize);
    }
}
