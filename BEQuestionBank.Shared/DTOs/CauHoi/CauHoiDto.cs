using System;
using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs.CauTraLoi;

namespace BEQuestionBank.Shared.DTOs.CauHoi
{
    public class CauHoiDto
    {
        public Guid MaCauHoi { get; set; }
        public Guid MaPhan { get; set; }
        public int MaSoCauHoi { get; set; }
        public string? NoiDung { get; set; }
        public bool HoanVi { get; set; }
        public short CapDo { get; set; }
        public int SoCauHoiCon { get; set; }
        public float? DoPhanCach { get; set; }
        public Guid? MaCauHoiCha { get; set; }
        public bool? XoaTam { get; set; }
        public int? SoLanDuocThi { get; set; }
        public int? SoLanDung { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public EnumCLO? CLO { get; set; }
        public List<CauTraLoiDto> CauTraLois { get; set; } = new List<CauTraLoiDto>();
    }
}