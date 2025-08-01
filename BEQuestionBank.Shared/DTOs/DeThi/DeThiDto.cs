using System;
using System.Collections.Generic;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;

namespace BEQuestionBank.Shared.DTOs.DeThi
{
    public class DeThiDto
    {
        public Guid MaDeThi { get; set; }
        public Guid MaMonHoc { get; set; }
        public string TenDeThi { get; set; }
        public bool? DaDuyet { get; set; }
        public int? SoCauHoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }
        public List<ChiTietDeThiDto> ChiTietDeThis { get; set; } = new List<ChiTietDeThiDto>();
    }
}