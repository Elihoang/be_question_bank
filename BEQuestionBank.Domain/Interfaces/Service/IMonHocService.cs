using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;
public interface IMonHocService : IService<MonHoc>
{
    Task<IEnumerable<MonHoc>> GetByMaKhoaAsync(Guid maKhoa);
}
