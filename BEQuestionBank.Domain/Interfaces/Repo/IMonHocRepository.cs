using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Repo;

public interface IMonHocRepository : IRepository<MonHoc>
{
    Task<IEnumerable<MonHoc>> GetByMaKhoaAsync(Guid maKhoa);
}