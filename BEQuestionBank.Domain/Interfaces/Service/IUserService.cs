using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Service
{
    public interface IUserService : IService<NguoiDung>
    {
        Task<NguoiDung> GetByIdAsync(Guid maNguoiDung); 
        Task<NguoiDung> CreateAsync(NguoiDung user);   
        Task<NguoiDung> UpdateAsync(Guid maNguoiDung, NguoiDung user); 
        Task<bool> DeleteAsync(Guid maNguoiDung);
        Task<NguoiDung> GetByUsernameAsync(string tenDangNhap);
    }
}
