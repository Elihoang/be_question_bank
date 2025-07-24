using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

public class Phan : ModelBase
{
    [Key]
    public Guid MaPhan { get; set; }
    public Guid MaMonHoc { get; set; }
    public string TenPhan { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public bool LaCauHoiNhom { get; set; } = false;

    public MonHoc? MonHoc { get; set; }
}
