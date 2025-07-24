namespace BEQuestionBank.Domain.Common;

public abstract class ModelBase
{
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public DateTime NgayCapNhap { get; set; } = DateTime.Now;
    public bool XoaTam { get; set; } = false;
}