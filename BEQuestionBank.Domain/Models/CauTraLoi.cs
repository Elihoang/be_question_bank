using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Domain.Models;

public class CauTraLoi
{
    [Key]
    public Guid MaCauTraLoi { get; set; }
    public Guid MaCauHoi { get; set; }
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public bool LaDapAn { get; set; }

    public CauHoi CauHoi { get; set; }
    public ICollection<FileDinhKem> FileDinhKems { get; set; }
}