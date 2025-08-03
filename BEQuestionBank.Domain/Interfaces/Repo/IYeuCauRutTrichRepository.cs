using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface IYeuCauRutTrichRepository : IRepository<YeuCauRutTrich>
    {
        Task<IEnumerable<YeuCauRutTrich>> GetByMaNguoiDungAsync(Guid maNguoiDung);
        Task<IEnumerable<YeuCauRutTrich>> GetByMaMonHocAsync(Guid maMonHoc);
        Task<IEnumerable<YeuCauRutTrich>> GetChuaXuLyAsync();
        Task<bool> ExistsNguoiDungAsync(Guid maNguoiDung);
        Task<bool> ExistsMonHocAsync(Guid maMonHoc);
    }
}