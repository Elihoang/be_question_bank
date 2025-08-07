using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Service
{
    public interface IYeuCauRutTrichService : IService<YeuCauRutTrich>
    {
        Task<IEnumerable<YeuCauRutTrichDto>> GetByMaNguoiDungAsync(Guid maNguoiDung);
        Task<IEnumerable<YeuCauRutTrichDto>> GetByMaMonHocAsync(Guid maMonHoc);
        Task<IEnumerable<YeuCauRutTrichDto>> GetChuaXuLyAsync();
        Task<YeuCauRutTrichDto> ChangerStatusAsync(Guid id, bool daXuLy);
    }
}