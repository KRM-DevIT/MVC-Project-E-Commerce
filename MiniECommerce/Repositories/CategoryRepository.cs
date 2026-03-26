
using Microsoft.EntityFrameworkCore;

namespace MiniECommerce.Repositories
{
    public class CategoryRepository : Repository<Category> , ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool CheckUniquness(string categoryname)
        {
           return _context.Categories.Any(c => c.CategoryName == categoryname);
        }

        public List<Category> CategoriesWithProducts()
        {
            return _context.Categories.Include(p=>p.Products).ToList();
        }

        public Category? CategoryByName(string Name)
        {
            return _context.Categories.Where(c=>c.CategoryName == Name).FirstOrDefault();
        }

        public List<Category> GetAllWithParent()
        {
            return _context.Categories.Include(c=>c.ParentCategory).ToList();
        }

        public Category? GetByIdWithParent(int id)
        {
            return _context.Categories
                .Where(c => c.CategoryId == id)
                .Include(c => c.ParentCategory)
                .FirstOrDefault();
        }

        public Category GetCategoryWithChildren(int id)
        {
            var category = _context.Categories
                        .Include(c => c.ChildrenCategories)
                        .First(c => c.CategoryId == id);

            return category;
        }
    }
}
