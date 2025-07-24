using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Domain.Models;

public class FileDinhKem
{
    [Key]
    public Guid MaFile { get; set; }
    public Guid? MaCauHoi { get; set; }
    public Guid? MaCauTraLoi { get; set; }
    public string? TenFile { get; set; }
    public string? DuongDan { get; set; }
    public int? LoaiFile { get; set; }

    public CauHoi? CauHoi { get; set; }
    public CauTraLoi? CauTraLoi { get; set; }
}