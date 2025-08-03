using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.Shared.DTOs;

public class UpdateYeuCauRutTrichDto
{
    [Required(ErrorMessage = "Mã người dùng không được để trống.")]
    public Guid MaNguoiDung { get; set; }

    [Required(ErrorMessage = "Mã môn học không được để trống.")]
    public Guid MaMonHoc { get; set; }

    public string? NoiDungRutTrich { get; set; }
    public string? GhiChu { get; set; }
    public DateTime? NgayYeuCau { get; set; }
    public DateTime? NgayXuLy { get; set; }
    public bool? DaXuLy { get; set; }
}