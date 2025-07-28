using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Shared.DTOs.CauHoi;
using System.ComponentModel.DataAnnotations;

public class UpdateCauHoiDto
{
    [Required(ErrorMessage = "Mã phần không được để trống.")]
    public Guid MaPhan { get; set; }

    [Required(ErrorMessage = "Mã số câu hỏi không được để trống.")]
    public int MaSoCauHoi { get; set; }

    public string NoiDung { get; set; }

    [Required(ErrorMessage = "Hoán vị không được để trống.")]
    public bool HoanVi { get; set; }

    [Required(ErrorMessage = "Cấp độ không được để trống.")]
    public short CapDo { get; set; }

    public int SoCauHoiCon { get; set; }

    public float? DoPhanCach{ get; set; }

    public Guid? MaCauHoiCha { get; set; }

    public bool? XoaTam { get; set; }

    public int? SoLanDuocThi { get; set; }

    public int? SoLanDung { get; set; }

    public EnumCLO? CLO { get; set; }
}
