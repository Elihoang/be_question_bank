using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;

public class CauTraLoi
{
    [Key] 
    public String MaCauTraLoi { get; set; } = CodeGenerator.GenerateTraLoiCode();
    public String MaCauHoi { get; set; }
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public bool LaDapAn { get; set; }

    public virtual CauHoi CauHoi { get; set; }
    public ICollection<FileDinhKem> FileDinhKems { get; set; }
}