
namespace MiniECommerce.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool CheckUniquness(string categoryname);
        List<Category> CategoriesWithProducts();

        Category? CategoryByName(string Name);
        List<Category> GetAllWithParent();
        Category? GetByIdWithParent(int id);
        Category GetCategoryWithChildren(int id);
    }
}
