namespace BEQuestionBank.Shared.DTOs;

public class MonHocDto
{
    public Guid MaMonHoc { get; set; }
    public string TenMonHoc { get; set; } = string.Empty;
    public Guid MaKhoa { get; set; }
    public bool? XoaTam { get; set; }
}
