using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface IDeThiRepository : IRepository<DeThi>
    {
        Task<DeThiDto> GetByIdWithChiTietAsync(Guid id);
        Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync();
        Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto);
        Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto);
        Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc);
        
        Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync();


    }
}