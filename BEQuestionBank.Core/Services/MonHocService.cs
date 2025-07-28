using System.Linq.Expressions;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Core.Services;

public class MonHocService(IMonHocRepository repository) : IMonHocService
{
    public async Task AddAsync(MonHoc entity)
    {
        await repository.AddAsync(entity);
    }

    public async Task DeleteAsync(MonHoc entity)
    {
        await repository.DeleteAsync(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<MonHoc, bool>> predicate)
    {
        return await repository.ExistsAsync(predicate);
    }

    public async Task<IEnumerable<MonHoc>> FindAsync(Expression<Func<MonHoc, bool>> predicate)
    {
        return await repository.FindAsync(predicate);
    }

    public async Task<MonHoc?> FirstOrDefaultAsync(Expression<Func<MonHoc, bool>> predicate)
    {
        return await repository.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<MonHoc>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<MonHoc?> GetByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MonHoc>> GetByMaKhoaAsync(Guid maKhoa)
    {
        return await repository.GetByMaKhoaAsync(maKhoa);
    }

    public async Task UpdateAsync(MonHoc entity)
    {
        await repository.UpdateAsync(entity);
    }
}
