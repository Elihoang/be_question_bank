using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface ICauHoiRepository : IRepository<CauHoi>
    {
        Task<IEnumerable<CauHoi>> GetAllWithAnswersAsync();
        Task<CauHoi> GetByIdWithAnswersAsync(string id);
        Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo);
        Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan);
        Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc);
        Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi);
        Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha); 
    }
}