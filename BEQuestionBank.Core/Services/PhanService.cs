using System.Linq.Expressions;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Core.Services;

public class PhanService : IPhanService
{
    private readonly IPhanRepository _repository;
    public PhanService(IPhanRepository repository)
    {
        _repository = repository;
    }
    public async Task<Phan> GetByIdAsync(Guid id)
    {
        return  await _repository.GetByIdAsync(id); 
    }

    public async Task<IEnumerable<Phan>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Phan>> FindAsync(Expression<Func<Phan, bool>> predicate)
    {
        return await  _repository.FindAsync(predicate);
    }

    public async Task<Phan> FirstOrDefaultAsync(Expression<Func<Phan, bool>> predicate)
    {
        return await _repository.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(Phan model)
    {
         await _repository.AddAsync(model);
    }

    public async Task UpdateAsync(Phan model)
    {
        await _repository.UpdateAsync(model);
    }

    public async Task DeleteAsync(Phan model)
    {
        await _repository.DeleteAsync(model);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Phan, bool>> predicate)
    {
        return await _repository.ExistsAsync(predicate);
    }

    public async Task<IEnumerable<Phan>> GetByMaMonHocAsync(Guid maMonHoc)
    {
        return await _repository.GetByMaMonHocAsync(maMonHoc);
    }
}