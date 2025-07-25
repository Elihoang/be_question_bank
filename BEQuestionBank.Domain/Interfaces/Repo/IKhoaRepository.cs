using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Repo;

public interface IKhoaRepository : IRepository<Khoa>
{
    Task<Khoa>  GetByTenKhoaAsync(string tenKhoa);
}