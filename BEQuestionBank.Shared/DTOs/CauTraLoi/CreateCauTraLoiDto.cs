using BEQuestionBank.Shared.DTOs.File;

namespace BEQuestionBank.Shared.DTOs.CauTraLoi;

public class CreateCauTraLoiDto
{
    public Guid MaCauHoi { get; set; }
    public string NoiDung { get; set; }
    public int ThuTu { get; set; }
    public bool LaDapAn { get; set; }
    public bool HoanVi { get; set; }
    
    public List<FileDto> Files { get; set; } = new List<FileDto>();
}