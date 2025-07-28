using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface IPhanService : IService<Phan>
{
    Task<IEnumerable<Phan>> GetByMaMonHocAsync(Guid maMonHoc);
}