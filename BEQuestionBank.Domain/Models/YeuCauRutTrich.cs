using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEQuestionBank.Domain.Models;

[Table("YeuCauRutTrich")]
public class YeuCauRutTrich
{
    [Key]
    public Guid MaYeuCau { get; set; }
    public Guid MaNguoiDung { get; set; }
    public Guid MaMonHoc { get; set; }
    public string? NoiDungRutTrich { get; set; }
    public string? GhiChu { get; set; }
    public DateTime? NgayYeuCau { get; set; }
    public DateTime? NgayXuLy { get; set; }
    public bool? DaXuLy { get; set; }
}
