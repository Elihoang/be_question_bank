using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface ICauHoiRepository : IRepository<CauHoi>
    {
        Task<IEnumerable<CauHoi>> GetAllWithAnswersAsync();
        Task<CauHoi> GetByIdWithAnswersAsync(string id);
    }
}