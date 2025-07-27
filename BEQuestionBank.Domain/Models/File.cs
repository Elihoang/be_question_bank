using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEQuestionBank.Domain.Models;

[Table(("Files"))]
public class File
{
    [Key] 
    public Guid MaFile { get; set; }
    public Guid? MaCauHoi { get; set; }
    public string? TenFile { get; set; }
    public int? LoaiFile { get; set; }
    public String? MaCauTraLoi { get; set; }

    public virtual CauHoi? CauHoi { get; set; }
    public virtual CauTraLoi? CauTraLoi { get; set; }
}