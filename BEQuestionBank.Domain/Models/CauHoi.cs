using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Domain.Models;

[Table("CauHoi")]
public class CauHoi : ModelBase
{
    [Key] 
    public Guid MaCauHoi { get; set; } = Guid.NewGuid();
    [ForeignKey("Phan")]
    public Guid MaPhan { get; set; }
    public int MaSoCauHoi { get; set; }
    public string? NoiDung { get; set; }
    public bool HoanVi { get; set; }
    public short CapDo { get; set; }
    public int SoCauHoiCon { get; set; }
    public Guid? MaCauHoiCha { get; set; }
    public bool TrangThai { get; set; } = false;
    public int? SoLanDuocThi { get; set; } = 0;
    public int? SoLanDung { get; set; } = 0;
    [Column("DoPhanCachCauHoi")]
    public float? DoPhanCach { get; set; }
    public bool? XoaTam { get; set; } = false;
    public EnumCLO? CLO { get; set; }
    public DateTime? NgaySua { get; set; }
    [ForeignKey("NguoiDung")]
    public Guid? NguoiTao { get; set; }

    public virtual Phan Phan { get; set; }
    public virtual CauHoi? CauHoiCha { get; set; }
    public ICollection<CauHoi> CauHoiCons { get; set; } = new List<CauHoi>();
    public virtual NguoiDung? NguoiDung { get; set; }
    public ICollection<CauTraLoi> CauTraLois { get; set; }
    public ICollection<File> Files { get; set; }
    public virtual ICollection<DeThi> DeThis { get; set; } = new List<DeThi>();
}