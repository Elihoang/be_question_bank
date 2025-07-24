namespace BEQuestionBank.Domain.Models;

public class ChiTietDeThi
{
    public Guid MaDeThi { get; set; }
    public Guid MaPhan { get; set; }
    public Guid MaCauHoi { get; set; }
    public int? ThuTu { get; set; }

    public DeThi DeThi { get; set; }
    public Phan Phan { get; set; }
    public CauHoi CauHoi { get; set; }
}