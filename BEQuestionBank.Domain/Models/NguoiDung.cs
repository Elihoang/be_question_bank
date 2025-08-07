using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Domain.Models;

[Table("NguoiDung")]
public class NguoiDung : ModelBase
{
    [Key]
    public Guid MaNguoiDung { get; set; }
    [Required]
    public string TenDangNhap { get; set; }
    [Required]
    public string MatKhau { get; set; }
    public string HoTen { get; set; }
    public string? Email { get; set; }
    public VaiTroNguoiDung VaiTro { get; set; }
    public bool BiKhoa { get; set; } = false;
    [ForeignKey("Khoa")]
    public Guid? MaKhoa { get; set; }

    public DateTime? NgayDangNhapCuoi { get; set; }

    public virtual Khoa? Khoa { get; set; }
    
    
}