using BEQuestionBank.Domain.Interfaces.Service;
using System.Linq.Expressions;
using BEQuestionBank.Domain.Interfaces.Repo;

namespace BEQuestionBank.Core.Services;

public class GenericService<T>(IRepository<T> repository) : IService<T>
    where T : class
{
    public Task<T> GetByIdAsync(Guid id) => repository.GetByIdAsync(id);

    public Task<IEnumerable<T>> GetAllAsync() => repository.GetAllAsync();

    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        repository.FindAsync(predicate);

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        repository.FirstOrDefaultAsync(predicate);

    public Task AddAsync(T entity) => repository.AddAsync(entity);

    public Task UpdateAsync(T entity) => repository.UpdateAsync(entity);

    public Task DeleteAsync(T entity) => repository.DeleteAsync(entity);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        repository.ExistsAsync(predicate);
}