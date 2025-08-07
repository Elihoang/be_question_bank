using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEQuestionBank.Domain.Models;

[Table("YeuCauRutTrich")]
public class YeuCauRutTrich
{
    [Key]
    public Guid MaYeuCau { get; set; } = Guid.NewGuid();
    [ForeignKey("NguoiDung")]
    public Guid MaNguoiDung { get; set; }
    [ForeignKey("MonHoc")]
    public Guid MaMonHoc { get; set; }
    public string? NoiDungRutTrich { get; set; }
    public string? GhiChu { get; set; }
    public DateTime? NgayYeuCau { get; set; }
    public DateTime? NgayXuLy { get; set; }
    public bool? DaXuLy { get; set; }
    
    public virtual MonHoc? MonHoc { get; set; }
    public virtual NguoiDung? NguoiDung { get; set; }
}
