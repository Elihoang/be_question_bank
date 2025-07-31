using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Domain.Models;

[Table(("Files"))]
public class File
{
    [Key] 
    public Guid MaFile { get; set; } = Guid.NewGuid();
    [ForeignKey("CauHoi")]
    public Guid? MaCauHoi { get; set; }
    public string? TenFile { get; set; }
    public FileType? LoaiFile { get; set; }
    [ForeignKey("CauTraLoi")]
    public Guid? MaCauTraLoi { get; set; }

    public virtual CauHoi? CauHoi { get; set; }
    public virtual CauTraLoi? CauTraLoi { get; set; }
}