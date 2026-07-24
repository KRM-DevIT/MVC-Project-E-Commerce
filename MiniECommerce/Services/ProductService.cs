using System.Security.Cryptography;

namespace MiniECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        private readonly IWebHostEnvironment _env;

        private const long MaxFileSizeBytes = 3 * 1024 * 1024; // 3MB in Bytes
        private const string ImagesFolder = "Images";
        private static readonly HashSet<string> AllowedExtensions
            = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

        public ProductService(IProductRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        // ── Image: Save ────────────────────────────────────────────────────────
        public async Task<(bool Success, string? FilePath, string? Error)> SaveImageAsync(IFormFile file)
        {
            // 1. Validate — null check
            if (file == null || file.Length == 0)
                return (false, null, "No file was uploaded.");

            // 2. Validate — file size
            if (file.Length > MaxFileSizeBytes)
                return (false, null, "Image must be 3 MB or smaller.");

            // 3. Validate — extension
            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                return (false, null, "Only .jpg, .jpeg, and .png files are allowed.");

            // 4. Build a unique file name to prevent collisions and path traversal
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // 5. Resolve physical path: wwwroot/images/
            var imagesDir = Path.Combine(_env.WebRootPath, ImagesFolder);
            Directory.CreateDirectory(imagesDir); // creates folder if it doesn't exist yet

            var fullPath = Path.Combine(imagesDir, uniqueFileName);

            // 6. Save to disk
            using (var stream = new FileStream(fullPath, FileMode.Create)) // like we open the folder where we will paste the image
            {
                await file.CopyToAsync(stream); // await is highly preferable here
            }

            // 7. Return the relative URL path for storing in the DB  e.g. /images/abc123.jpg
            return (true, $"/{ImagesFolder}/{uniqueFileName}", null);
        }

        // ── Image: Delete ──────────────────────────────────────────────────────
        public void DeleteImage(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            // Convert relative URL  /images/abc.jpg  →  physical path
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/')); // because the webrootpath add / at end

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public bool CreateProduct(Product product)
        {
            bool productExists = _repository.CheckUniquness(product.SKU!);

            if (productExists)
                return false;

            _repository.Insert(product);

            return true;
        }

        public bool DeleteProduct(int id)
        {
            var productToBeDeleted = _repository.GetById(id);

            if (productToBeDeleted == null)
                return false;

            _repository.Delete(productToBeDeleted);

            return true;
        }

        public List<Product> GetAllProducts() // Browse
        {
           return _repository.GetAll();
        }

        public Product? GetProductById(int id)
        {
            return _repository.GetById(id);
        }

        public List<Product> GetProductsByCategory(int categoryId) // filterbycategory
        {
            return _repository.ProductsByCategory(categoryId);
        }


        public List<Product> GetProductsWithPagination(int pageNumber, int pageSize, bool activeOnly = false)
        {
            return _repository.ProductsPaginated( pageNumber, pageSize, activeOnly);
        }

        public int GetProductCount(bool activeOnly = false)
        {
            return _repository.GetProductsTotalCount(activeOnly);
        }

        public List<Product> SearchProductsByKeyword(string keyword) // search
        {
            return _repository.SearchProducts(keyword);
        }

        public void UpdateProduct(Product Product)
        {

                _repository.Update(Product);
       
        }

        public List<Product> GetProductsByIDs(List<int> productIds)
        {
            return _repository.FilterProductsByIds(productIds);
        }

        public string GenerateUniqueSKU(string productName, string categoryName)
        {
            string categoryPart = categoryName.ToUpper()
                .Replace(" ", "")
                .Replace("-", "")
                ?? "GEN";

            categoryPart = categoryPart.Length > 4
                ? categoryPart.Substring(1,5)
                : categoryPart.PadRight(4, 'X');

            string namePart = productName
                .ToUpper()
                .Replace(" ", "")
                .Replace("-", "");

            namePart = namePart.Length > 5
                ? namePart.Substring(1,5)
                : namePart.PadRight(5, 'X');

            int suffix = RandomNumberGenerator.GetInt32(10000);

            return $"{categoryPart}-{namePart}-{suffix}";
        }

        public List<Product> GetProductsByCategoryWithPagination(List<int> categoryIds, int pageNumber, int pageSize)
        {
            return _repository.GetProductsByCategoryWithPagination(categoryIds, pageNumber, pageSize);
        }

        public int GetProductsByCategoryCount(List<int> categoryIds)
        {
            return _repository.GetProductsByCategoryCount(categoryIds);
        }

        public int GetSearchProductCount(List<int> selectedcategories, string query)
        {
            return _repository.GetSearchProductCount(selectedcategories, query);
        }

        public List<Product> SearchProductsWithPagination(List<int> selectedcategories, string query, int pageNumber, int pageSize)
        {
            return _repository.SearchProductsWithPagination(selectedcategories, query, pageNumber, pageSize);
        }

        public Product? GetProductWithCategory(int productId)
        {
            return _repository.GetProductWithCategory(productId);
        }
    }
}
