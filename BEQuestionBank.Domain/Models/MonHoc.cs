using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

public class MonHoc : ModelBase
{
    public Guid MaMonHoc { get; set; }
    public string TenMonHoc { get; set; } = string.Empty;
    public string MaSoMonHoc { get; set; } = string.Empty;

    public Guid MaKhoa { get; set; }
    public Khoa? Khoa { get; set; }

    // Navigation
    public ICollection<Phan> DanhSachPhan { get; set; } = new List<Phan>();
}
