using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

[Table("DeThi")]
public class DeThi : ModelBase
{
    [Key] 
    public Guid MaDeThi { get; set; }
    public Guid MaMonHoc { get; set; }
    public string TenDeThi { get; set; } = string.Empty;
    public bool DaDuyet { get; set; } = false;
    public int? SoCauHoi { get; set; }

    public virtual MonHoc MonHoc { get; set; }
    public ICollection<ChiTietDeThi> ChiTietDeThis { get; set; }
}