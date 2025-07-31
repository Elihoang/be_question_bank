using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Shared.DTOs.File;

public class FileDto
{
    public Guid MaFile { get; set; }
    public Guid? MaCauHoi { get; set; }
    public Guid? MaCauTraLoi { get; set; }
    public string TenFile { get; set; }
    public FileType? LoaiFile { get; set; }
    
}