using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;

public class Phan : ModelBase
{
    [Key]
    public String MaPhan { get; set; } = CodeGenerator.GeneratePhanCode();
    public String MaMonHoc { get; set; }
    public string TenPhan { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public bool LaCauHoiNhom { get; set; } = false;

    public virtual MonHoc? MonHoc { get; set; }
}
