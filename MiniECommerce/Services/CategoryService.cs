using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniECommerce.Services
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
            
               return true;
            
        }

        public bool DeleteCategory(int id)
        {
           var categoryTobeDeleted = _repository.GetCategoryWithChildren(id); 
            // due to setclientNULL better use restrict or setnull next time

            if (categoryTobeDeleted == null) 
                return false;

            _repository.Delete(categoryTobeDeleted);
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

        public void UpdateCategory(Category category)
        {
  
                _repository.Update(category);
  
        }

        public List<SelectListItem> CategoryDropDownList()
        {
            var categories = _repository.GetAll();
            var CategorySelectListItem = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return CategorySelectListItem;
        }

        public List<Category> GetAllCategoriesWithParent()
        {
           return _repository.GetAllWithParent();
        }

        public Category? GetCategoryByIdWithParent(int id)
        {
           return _repository.GetByIdWithParent(id);
        }
    }
}
