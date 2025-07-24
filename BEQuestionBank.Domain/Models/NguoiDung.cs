using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Domain.Models;

public class NguoiDung : ModelBase
{
    [Key]
    public Guid MaNguoiDung { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? Email { get; set; }
    public VaiTroNguoiDung VaiTro { get; set; }
    public bool BiKhoa { get; set; } = false;
    public String? MaKhoa { get; set; }

    public virtual Khoa? Khoa { get; set; }
    public ICollection<GiangVienMonHoc> GiangVienMonHocs { get; set; }
    public ICollection<CauHoi> CauHois { get; set; }
}