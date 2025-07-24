using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

public class Khoa : ModelBase
{
    public Guid MaKhoa { get; set; }
    public string TenKhoa { get; set; } = string.Empty;

    // Navigation
    public ICollection<MonHoc> DanhSachMonHoc { get; set; } = new List<MonHoc>();
}