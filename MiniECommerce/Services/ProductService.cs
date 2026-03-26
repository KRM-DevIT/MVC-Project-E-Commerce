namespace MiniECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        public bool CreateProduct(Product product)
        {
            bool ProductExists = _repository.CheckUniquness(product.SKU!);
            if (ProductExists)
            {
                return false;
            }

            try
            {
                _repository.Insert(product);
                _repository.Save();
                return true;
            }
            catch {
                return false;
            }
        }

        public bool DeleteProduct(int id)
        {
            var productTobeDeleted = _repository.GetById(id);

            if (productTobeDeleted == null)
                return false;
            try
            {
                _repository.Delete(productTobeDeleted);
                _repository.Save();
                return true;
            }
            catch
            {
                return false;
            }
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

        public List<Product> GetProductsWithPagination(int pageNumber, int pageSize) // pagination
        {
            return _repository.ProductsPaginated(pageNumber, pageSize);
            
        }

        public List<Product> SearchProductsByKeyword(string keyword) // search
        {
            return _repository.SearchProducts(keyword);
        }

        public bool UpdateProduct(Product Product)
        {
            try
            {
                _repository.Update(Product);
                _repository.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Product> GetProductsByIDs(List<int> productIds)
        {
            return _repository.FilterProductsByIds(productIds);
        }

        public int GetProductCount()
        {
            return _repository.GetProductsTotalCount();
        }
    }
}
