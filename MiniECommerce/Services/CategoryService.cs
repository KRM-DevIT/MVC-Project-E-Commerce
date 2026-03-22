
namespace MiniECommerce.Interfaces.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _repository = categoryRepository;
        }

        public bool CreateNewCategory(Category category)
        {
            var categoryExits = _repository.CheckUniquness(category.CategoryName);

            if (categoryExits)
            {
                return false; // Not Unique Name
            }
             _repository.Insert(category);
             _repository.Save();
            
               return true;
            
        }

        public bool DeleteCategory(int id)
        {
            var categoryTobeDeleted = _repository.GetById(id);
            
            if (categoryTobeDeleted == null) 
                return false;

            _repository.Delete(categoryTobeDeleted);
            _repository.Save();
            return true;

        }

        public List<Category> GetAllCategories()
        {
            return _repository.GetAll();
        }

        public List<Category> GetCategoriesWithProducts()
        {
            return _repository.CategoriesWithProducts();
        }

        public Category? GetCategoryById(int id)
        {
            return _repository.GetById(id);
        }

        public Category? GetCategoryByName(string categoryName)
        {
            return _repository.CategoryByName(categoryName);
        }

        public bool UpdateCategory(Category category)
        {
            try
            {
                _repository.Update(category);
                _repository.Save();
                return true;
            }
            catch  { return false;     }
        }
    }
}
