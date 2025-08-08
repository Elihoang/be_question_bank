using BEQuestionBank.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Shared.DTOs.user
{
    public class NguoiDungDto
    {
        public Guid MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string ?TenKhoa { get; set; }
        public string HoTen { get; set; }
        public string? Email { get; set; }
        public VaiTroNguoiDung VaiTro { get; set; }
        public bool BiKhoa { get; set; }
        public DateTime? NgayDangNhapCuoi { get; set; }
    }
}
