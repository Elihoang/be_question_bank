using System;
using System.Collections.Generic;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using BEQuestionBank.Shared.DTOs.File;

namespace BEQuestionBank.Shared.DTOs.CauHoi
{
    public class CreateCauHoiWithAnswersDto
    {
        public Guid MaPhan { get; set; }
        public int MaSoCauHoi { get; set; }
        public string NoiDung { get; set; }
        public bool HoanVi { get; set; }
        public short CapDo { get; set; }
        public int SoCauHoiCon { get; set; }
        public float? DoPhanCach { get; set; }
        public Guid? MaCauHoiCha { get; set; }
        public bool? XoaTam { get; set; } = false;
        public int? SoLanDuocThi { get; set; } = 0;
        public int? SoLanDung { get; set; } = 0;
        public EnumCLO? CLO { get; set; }
        public List<CreateCauTraLoiDto> CauTraLois { get; set; } = new List<CreateCauTraLoiDto>();
        public List<FileDto> Files { get; set; } = new List<FileDto>();
    }
}