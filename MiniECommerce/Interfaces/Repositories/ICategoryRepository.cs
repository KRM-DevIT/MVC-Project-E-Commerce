
namespace MiniECommerce.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool CheckUniquness(string categoryname);
        List<Category> CategoriesWithProducts();

        Category? CategoryByName(string Name);
    }
}
