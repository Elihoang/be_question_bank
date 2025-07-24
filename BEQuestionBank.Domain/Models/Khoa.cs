using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Common;
using BEQuestionBank.Shared.Helpers;

namespace BEQuestionBank.Domain.Models;

public class Khoa : ModelBase
{
    [Key]
    public String MaKhoa { get; set; } = CodeGenerator.GenerateKhoaCode();
    public string TenKhoa { get; set; } = string.Empty;

    // Navigation
    public ICollection<MonHoc> DanhSachMonHoc { get; set; } = new List<MonHoc>();
}