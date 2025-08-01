using System.ComponentModel;
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
    [DefaultValue("")]
    public string? MoTa { get; set; }

    [Column("XoaTamKhoa")]
    public bool? XoaTam { get; set; } = true;

    // Navigation
    public ICollection<MonHoc> DanhSachMonHoc { get; set; } = new List<MonHoc>();
}