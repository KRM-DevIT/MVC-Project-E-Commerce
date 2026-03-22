namespace MiniECommerce.Interfaces.Services
{
    public interface ICategoryService
    {
        Category? GetCategoryByName(string categoryName);
        Category? GetCategoryById(int id);
        List<Category> GetAllCategories();
        bool CreateNewCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int id);
        List<Category> GetCategoriesWithProducts();

    }
}
