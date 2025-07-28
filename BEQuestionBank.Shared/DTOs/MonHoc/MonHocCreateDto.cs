namespace BEQuestionBank.Shared.DTOs;

public class MonHocCreateDto
{
    public string TenMonHoc { get; set; } = string.Empty;
    public Guid MaKhoa { get; set; }
    public bool? XoaTam { get; set; }
}
