using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;


public class DeThi : ModelBase
{
    public Guid MaDeThi { get; set; }
    public Guid MaMonHoc { get; set; }
    public string TenDeThi { get; set; } = string.Empty;
    public bool DaDuyet { get; set; } = false;
    public int? SoCauHoi { get; set; }

    public MonHoc MonHoc { get; set; }
    public ICollection<ChiTietDeThi> ChiTietDeThis { get; set; }
}