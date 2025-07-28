using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface ICauTraLoiService : IService<CauTraLoi>
{
    Task<IEnumerable<CauTraLoi>> GetByMaCauHoi(Guid maCauHoi);
}