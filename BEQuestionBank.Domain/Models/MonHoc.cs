using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

[Table("MonHoc")]
public class MonHoc : ModelBase
{
    [Key]
    public Guid MaMonHoc { get; set; }
    public string TenMonHoc { get; set; } = string.Empty;
    public string MaSoMonHoc { get; set; } = string.Empty;
    public int? SoTinChi { get; set; }
    [Column("XoaTamMonHoc")]
    public bool? XoaTam { get; set; } = false;
    public Guid MaKhoa { get; set; }
    public Khoa? Khoa { get; set; }

    // Navigation
    public ICollection<Phan> DanhSachPhan { get; set; } = new List<Phan>();
}
