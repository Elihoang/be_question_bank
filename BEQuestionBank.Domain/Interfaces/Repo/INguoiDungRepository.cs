using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface INguoiDungRepository : IRepository<NguoiDung>
    {
        Task<IEnumerable<NguoiDung>> GetByVaiTroAsync(VaiTroNguoiDung vaiTro);
        Task<IEnumerable<NguoiDung>> GetByKhoaAsync(Guid maKhoa);
        Task<bool> IsLockedAsync(string tenDangNhap);
        Task<NguoiDung> GetByResetCodeAsync(Guid maKhoa);
    }
}
