using Microsoft.EntityFrameworkCore;
namespace HotelManagementSystem.Infrastructure;
public interface IRepository<T> where T: class { IQueryable<T> Query(); Task<T?> GetAsync(int id); Task AddAsync(T entity); void Update(T entity); void Remove(T entity); Task<int> SaveAsync(); }
public class Repository<T>(HmsDbContext db) : IRepository<T> where T: class { public IQueryable<T> Query()=>db.Set<T>(); public Task<T?> GetAsync(int id)=>db.Set<T>().FindAsync(id).AsTask(); public Task AddAsync(T e)=>db.Set<T>().AddAsync(e).AsTask(); public void Update(T e)=>db.Set<T>().Update(e); public void Remove(T e)=>db.Set<T>().Remove(e); public Task<int> SaveAsync()=>db.SaveChangesAsync(); }
