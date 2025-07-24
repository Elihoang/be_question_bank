using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Domain.Models;

public class ChiTietDeThi
{
    [Key]
    public String MaDeThi { get; set; }
    public String MaPhan { get; set; }
    public String MaCauHoi { get; set; }
    public int? ThuTu { get; set; }

    public virtual DeThi DeThi { get; set; }
    public virtual Phan Phan { get; set; }
    public virtual CauHoi CauHoi { get; set; }
}