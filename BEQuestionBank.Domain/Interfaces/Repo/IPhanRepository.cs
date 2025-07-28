using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Repo;

public interface IPhanRepository : IRepository<Phan>
{
    Task<IEnumerable<Phan>> GetByMaMonHocAsync(Guid maMonHoc);
}