using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;


public class DeThi : ModelBase
{
    [Key] 
    public String MaDeThi { get; set; } = CodeGenerator.GenerateDeThiCode();
    public String MaMonHoc { get; set; }
    public string TenDeThi { get; set; } = string.Empty;
    public bool DaDuyet { get; set; } = false;
    public int? SoCauHoi { get; set; }

    public virtual MonHoc MonHoc { get; set; }
    public ICollection<ChiTietDeThi> ChiTietDeThis { get; set; }
}