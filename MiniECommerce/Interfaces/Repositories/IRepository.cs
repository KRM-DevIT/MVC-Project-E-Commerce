namespace MiniECommerce.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T? GetById(int id);
        List<T> GetAll();
        void Insert(T item);
        void Update(T item);    
        void Delete(T item);
        //void Save();
    }
}
