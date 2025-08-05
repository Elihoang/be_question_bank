// BEQuestionBank.Shared.DTOs.DeThi/DeThiWithChiTietAndCauTraLoiDto.cs
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using System;
using System.Collections.Generic;

namespace BEQuestionBank.Shared.DTOs.DeThi
{
    public class DeThiWithChiTietAndCauTraLoiDto
    {
        public Guid MaDeThi { get; set; }
        public Guid MaMonHoc { get; set; }
        public string TenDeThi { get; set; }
        public bool DaDuyet { get; set; }
        public int SoCauHoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhap { get; set; }
        public List<ChiTietDeThiDto> ChiTietDeThis { get; set; }
        public Dictionary<Guid, List<CauTraLoiDto>> CauTraLoiByCauHoi { get; set; } 
    }
}