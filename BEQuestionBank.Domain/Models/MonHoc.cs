using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

public class MonHoc : ModelBase
{
    [Key]
    public String MaMonHoc { get; set; }
    public string TenMonHoc { get; set; } = string.Empty;
    public string MaSoMonHoc { get; set; } = string.Empty;
    public int SoTinChi { get; set; }
    public String MaKhoa { get; set; }
    public Khoa? Khoa { get; set; }

    // Navigation
    public ICollection<Phan> DanhSachPhan { get; set; } = new List<Phan>();
}
