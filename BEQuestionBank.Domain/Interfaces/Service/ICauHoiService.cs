using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;

namespace BEQuestionBank.Domain.Interfaces.Service;

public interface ICauHoiService : IService<CauHoi>
{
    Task<IEnumerable<CauHoiDto>> GetAllWithAnswersAsync();
    Task<CauHoiDto> GetByIdWithAnswersAsync(Guid maCauHoi);
    Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo);
    Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan);
    Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc);
    Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi);
    Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha); 
    Task<IEnumerable<CauHoiDto>> GetAllGroupsAsync();
}