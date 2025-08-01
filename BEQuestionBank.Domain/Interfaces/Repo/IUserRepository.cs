using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface IUserRepository : IRepository<NguoiDung>
    {
        Task<NguoiDung> GetByIdAsync(Guid maNguoiDung);
        Task<NguoiDung> GetByUsernameAsync(string tenDangNhap);
    }
}
