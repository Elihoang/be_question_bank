using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface ICauTraLoiService : IService<CauTraLoi>
{
    Task<List<CauTraLoi>> GetByMaCauHoi(Guid maCauHoi);
}