namespace BEQuestionBank.Shared.DTOs.Khoa;

public class KhoaDto
{
    public Guid MaKhoa { get; set; }
    public string TenKhoa { get; set; }
    public bool? XoaTam { get; set; }
    public List<MonHocDto> DanhSachMonHoc { get; set; }


}
