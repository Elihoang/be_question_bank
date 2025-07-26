using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;

[Table("Phan")]
public class Phan : ModelBase
{
    [Key]
    public Guid MaPhan { get; set; }
    public Guid MaMonHoc { get; set; }
    [Required]
    public string TenPhan { get; set; }
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public int SoLuongCauHoi  { get; set; }
    public Guid? MaPhanCha { get; set; }
    public int? MaSoPhan { get; set; }
    [Column("XoaTamPhan")]
    public bool? XoaTam { get; set; }
    public bool LaCauHoiNhom { get; set; } = false;

    public virtual MonHoc? MonHoc { get; set; }
}
