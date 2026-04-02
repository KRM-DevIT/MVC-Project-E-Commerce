using Microsoft.EntityFrameworkCore;
using System;

namespace MiniECommerce.Repositories
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        // Here we add functions that aren't just simple CRUD But talks to DB as well and don't forget to add it to the IProductRepository.

        public bool CheckUniquness(string SKU)
        {
           return _context.Products.Any(p=>p.SKU == SKU);   
        }

        public List<Product> ProductsByCategory(int categoryId)
        {
           return _context.Products.Where(p=>p.CategoryId== categoryId).ToList(); 
        }

        public List<Product> ProductsPaginated(int pageNumber, int pageSize)
        {
            return _context.Products
                           .OrderBy(p => p.ProductId)
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }

        public List<Product> SearchProducts(string keyword)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(p =>
                p.ProductName.Contains(keyword) ||
                p.SKU!.Contains(keyword)
            );

            if (decimal.TryParse(keyword, out decimal price))
            {
                query = query.Where(p => p.CurrentPrice == price);
            }

            return query.ToList();
        }

        public List<Product> FilterProductsByIds(List<int> productIds)
        {
            return _context.Products.Where(p=>productIds.Contains(p.ProductId)).ToList();
        }

        public int GetProductsTotalCount()
        {
            return _context.Products.Count();
        }

        public int GetProductsByCategoryCount(List<int> categoryIds)
        {
            return _context.Products
                            .Where(p => categoryIds.Contains(p.CategoryId))
                            .Count();
        }

        public List<Product> GetProductsByCategoryWithPagination(List<int> categoryIds, int pageNumber, int pageSize)
        {
            return _context.Products
                           .Where(p => categoryIds.Contains(p.CategoryId))
                           .OrderBy(p => p.ProductId)
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .AsNoTracking()
                           .ToList();
        }

        public int GetSearchProductCount(List<int> selectedcategories, string query)
        {
            bool HasCategoryFilter = selectedcategories != null && selectedcategories.Count > 0;
            decimal? priceQuery;

            if (decimal.TryParse(query, out var parsed))
            {
                priceQuery = parsed;
            }
            else
            {
                priceQuery = null;
            }

            return _context.Products.Where(  p => (!HasCategoryFilter || selectedcategories!.Contains(p.CategoryId)) &&
                                            (p.ProductName.Contains(query) ||
                                             p.SKU!.Contains(query) ||
                                            (priceQuery.HasValue && p.CurrentPrice >= priceQuery.Value && p.CurrentPrice < priceQuery.Value + 1))
                                          )
                                      .Count();
        }

        public List<Product> SearchProductsWithPagination(List<int> selectedcategories, string query, int pageNumber, int pageSize)
        {
            bool HasCategoryFilter = selectedcategories != null && selectedcategories.Count > 0;
            decimal? priceQuery;

            if (decimal.TryParse(query, out var parsed))
            {
                priceQuery = parsed;
            }
            else
            {
                priceQuery = null;
            }

            return _context.Products.Where(  p => (!HasCategoryFilter || selectedcategories!.Contains(p.CategoryId)) &&
                                            (p.ProductName.Contains(query) ||
                                             p.SKU!.Contains(query) ||
                                             p.Description!.Contains(query) ||
                                            (priceQuery.HasValue && p.CurrentPrice >= priceQuery.Value && p.CurrentPrice < priceQuery.Value + 1))
                                          ) 
                                   .OrderBy(p => p.ProductId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToList();
        }
    }
}
