using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Shared.DTOs.user
{
    public class ResetPasswordDto
    {
        public string TenDangNhap { get; set; }
        public Guid MaKhoa { get; set; }
        public string NewPassword { get; set; }
    }
}
