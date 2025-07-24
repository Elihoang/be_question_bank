using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

public class CauHoi : ModelBase
{
    public Guid MaCauHoi { get; set; }
    public Guid MaPhan { get; set; }
    public int MaSoCauHoi { get; set; }
    public string? NoiDung { get; set; }
    public bool HoanVi { get; set; }
    public short CapDo { get; set; }
    public bool LaCauHoiNhom { get; set; }
    public Guid? MaCauHoiCha { get; set; }
    public bool TrangThai { get; set; } = false;
    public int SoLanDuocThi { get; set; } = 0;
    public int SoLanDung { get; set; } = 0;
    public float? DoPhanCach { get; set; }
    public DateTime? NgaySua { get; set; }
    public Guid? NguoiTao { get; set; }

    public Phan Phan { get; set; }
    public CauHoi? CauHoiCha { get; set; }
    public ICollection<CauHoi> CauHoiCons { get; set; }
    public NguoiDung? NguoiDung { get; set; }
    public ICollection<CauTraLoi> CauTraLois { get; set; }
    public ICollection<FileDinhKem> FileDinhKems { get; set; }
}