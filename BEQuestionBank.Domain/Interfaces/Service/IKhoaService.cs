using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface IKhoaService : IService<Khoa>
{
    Task<Khoa> GetByTenKhoaAsync (string tenKhoa);
}