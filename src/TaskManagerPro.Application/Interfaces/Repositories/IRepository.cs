using System.Linq;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

