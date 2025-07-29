using BEQuestionBank.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface ICauTraLoiRepository : IRepository<CauTraLoi>
    {
        Task<IEnumerable<CauTraLoi>> GetByMaCauHoi(Guid maCauHoi);
        
    }
}