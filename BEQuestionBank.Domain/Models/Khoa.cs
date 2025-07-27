using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Common;

namespace BEQuestionBank.Domain.Models;

[Table("Khoa")]
public class Khoa : ModelBase
{
    [Key]
    public Guid MaKhoa { get; set; } = Guid.NewGuid();
    public string TenKhoa { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;

    [Column("XoaTamKhoa")]
    public bool? XoaTam { get; set; } = true;

    // Navigation
    public ICollection<MonHoc> DanhSachMonHoc { get; set; } = new List<MonHoc>();
}