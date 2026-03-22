using Microsoft.EntityFrameworkCore;
using MiniECommerce.Interfaces.Repositories;
using System;

namespace ecommerce.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); //if we pass <Product> -> _dbSet = context.Products 
        }

        public T? GetById(int id) // the ? incase not-Found
        {
            return _dbSet.Find(id);
        }     

        public List<T> GetAll()
        {
            return _dbSet.AsNoTracking<T>().ToList();
        }

        public void Insert(T item)
        {
            _dbSet.Add(item);
        }

        public void Update(T item)
        {
            _dbSet.Update(item);
        }

        public void Delete(T item)
        {
            _dbSet.Remove(item);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}