namespace BEQuestionBank.Application.DTOs
{
    public class PhanDto
    {
        public Guid MaPhan { get; set; }
        public Guid MaMonHoc { get; set; }
        public string TenPhan { get; set; } = string.Empty;
        public string? NoiDung { get; set; }
        public int ThuTu { get; set; }
        public int SoLuongCauHoi { get; set; }
        public Guid? MaPhanCha { get; set; }
        public int? MaSoPhan { get; set; }
        public bool? XoaTam { get; set; }
        public bool LaCauHoiNhom { get; set; }
        public string? TenMonHoc { get; set; }
    }
}