using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Domain.Models;

public class AuditLog
{
    [Key]
    public long MaNhatKy { get; set; }
    public string TenBang { get; set; } = string.Empty;
    public string MaBanGhi { get; set; } = string.Empty;
    public string HanhDong { get; set; } = string.Empty;
    public string? GiaTriCu { get; set; }
    public string? GiaTriMoi { get; set; }
    public Guid? MaNguoiDung { get; set; }
    public string? TenNguoiDung { get; set; }
    public DateTime ThoiGianThucHien { get; set; } = DateTime.Now;
    public string? DiaChiIP { get; set; }
    public string? UserAgent { get; set; }
    public string? GhiChu { get; set; }

    public NguoiDung? NguoiDung { get; set; }
}