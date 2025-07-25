using System.Linq.Expressions;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.Helpers;
using BEQuestionBank.Shared.Logging;

namespace BEQuestionBank.Core.Services;

public class KhoaService : IKhoaService
{
    private readonly IKhoaRepository _repository;

    public KhoaService(IKhoaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Khoa> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Khoa with id {id} not found");
    }

    public async Task<IEnumerable<Khoa>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Khoa>> FindAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _repository.FindAsync(predicate);
    }

    public async Task<Khoa> FirstOrDefaultAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _repository.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(Khoa model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(model.TenKhoa))
            throw new ArgumentException("Tên Khoa là bắt buộc.", nameof(model.TenKhoa));

        if (string.IsNullOrWhiteSpace(model.MaKhoa))
        {
            string newId;
            do
            {
                newId = CodeGenerator.GenerateKhoaCode();
            }
            while (await _repository.ExistsAsync(k => k.MaKhoa == newId));
            model.MaKhoa = newId;
        }

        await _repository.AddAsync(model);
    }

    public async Task AddRangeAsync(IEnumerable<Khoa> entities)
    {
        await _repository.AddRangeAsync(entities);
    }

    public async Task UpdateAsync(Khoa entity)
    {
        await _repository.UpdateAsync(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<Khoa> entities)
    {
        await _repository.UpdateRangeAsync(entities);
    }

    public async Task DeleteAsync(Khoa entity)
    {
        await _repository.DeleteAsync(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<Khoa> entities)
    {
        await _repository.DeleteRangeAsync(entities);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _repository.ExistsAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<Khoa, bool>> predicate = null)
    {
        return await _repository.CountAsync(predicate);
    }

    public async Task<Khoa> GetByTenKhoaAsync(string tenKhoa)
    {
        return await _repository.GetByTenKhoaAsync(tenKhoa);
    }
}