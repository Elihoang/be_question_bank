using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using System.Linq.Expressions;

namespace BEQuestionBank.Core.Services;

public class KhoaService(IKhoaRepository khoaRepository) : IKhoaService
{
    public Task<Khoa> GetByIdAsync(Guid id) => khoaRepository.GetByIdAsync(id);

    public Task<IEnumerable<Khoa>> GetAllAsync() => khoaRepository.GetAllAsync();

    public Task<IEnumerable<Khoa>> FindAsync(Expression<Func<Khoa, bool>> predicate) =>
        khoaRepository.FindAsync(predicate);

    public Task<Khoa> FirstOrDefaultAsync(Expression<Func<Khoa, bool>> predicate) =>
        khoaRepository.FirstOrDefaultAsync(predicate);

    public Task AddAsync(Khoa entity) => khoaRepository.AddAsync(entity);

    public Task UpdateAsync(Khoa entity) => khoaRepository.UpdateAsync(entity);

    public Task DeleteAsync(Khoa entity) => khoaRepository.DeleteAsync(entity);

    public Task<bool> ExistsAsync(Expression<Func<Khoa, bool>> predicate) =>
        khoaRepository.ExistsAsync(predicate);

    public Task<Khoa> GetByTenKhoaAsync(string tenKhoa) =>
        khoaRepository.GetByTenKhoaAsync(tenKhoa);
}