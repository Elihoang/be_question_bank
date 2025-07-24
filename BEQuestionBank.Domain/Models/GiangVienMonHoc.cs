using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Domain.Models;

public class GiangVienMonHoc
{
    [Key]
    public Guid MaNguoiDung { get; set; }
    public String MaMonHoc { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public string? GhiChu { get; set; }

    public virtual NguoiDung NguoiDung { get; set; }
    public virtual MonHoc MonHoc { get; set; }
}
