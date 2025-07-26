using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEQuestionBank.Domain.Models;

[Table("ChiTietDeThi")]
public class ChiTietDeThi
{
    [Key]
    public Guid MaDeThi { get; set; }
    [Required]
    public Guid MaPhan { get; set; }
    [Required]
    public Guid MaCauHoi { get; set; }
    public int? ThuTu { get; set; }

    public virtual DeThi DeThi { get; set; }
    public virtual Phan Phan { get; set; }
    public virtual CauHoi CauHoi { get; set; }
}