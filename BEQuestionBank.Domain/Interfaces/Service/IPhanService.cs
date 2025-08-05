using BEQuestionBank.Application.DTOs;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface IPhanService : IService<Phan>
{
    Task<IEnumerable<PhanDto>> GetByMaMonHocAsync(Guid maMonHoc);
}