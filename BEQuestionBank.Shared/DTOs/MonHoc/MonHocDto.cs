namespace BEQuestionBank.Shared.DTOs;

public class MonHocDto
{
    public Guid MaMonHoc { get; set; }
    public string MaSoMonHoc { get; set; }
    public string TenMonHoc { get; set; } = string.Empty;
    public int? SoTinChi { get; set; }
    public Guid MaKhoa { get; set; }
    public bool? XoaTam { get; set; }
}
