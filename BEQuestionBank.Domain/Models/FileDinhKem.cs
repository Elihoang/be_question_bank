using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;

public class FileDinhKem
{
    [Key] 
    public String MaFile { get; set; } = CodeGenerator.GenerateFileCode();
    public String? MaCauHoi { get; set; }
    public String? MaCauTraLoi { get; set; }
    public string? TenFile { get; set; }
    public string? DuongDan { get; set; }
    public int? LoaiFile { get; set; }

    public virtual CauHoi? CauHoi { get; set; }
    public virtual CauTraLoi? CauTraLoi { get; set; }
}