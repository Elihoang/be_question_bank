using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface ICauHoiService : IService<CauHoi>
{
    Task<IEnumerable<Object>> GetAllWithAnswersAsync();
}