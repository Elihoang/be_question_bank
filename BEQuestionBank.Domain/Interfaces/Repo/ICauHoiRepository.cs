using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using Microsoft.EntityFrameworkCore.Storage;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface ICauHoiRepository : IRepository<CauHoi>
    {
        Task<IEnumerable<CauHoiDto>> GetAllWithAnswersAsync();
        Task<CauHoiDto> GetByIdWithAnswersAsync(Guid maCauHoi);
        Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo);
        Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan);
        Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc);
        Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi);
        Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha); 
        Task<IEnumerable<CauHoiDto>> GetAllGroupsAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<CauHoiDto> AddWithAnswersAsync(CreateCauHoiWithAnswersDto cauHoiDto);
        Task<CauHoiDto> UpdateWithAnswersAsync(Guid maCauHoi, UpdateCauHoiWithAnswersDto cauHoiDto);
    }
}