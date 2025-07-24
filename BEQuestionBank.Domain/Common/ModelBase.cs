namespace BEQuestionBank.Domain.Common;

public abstract class ModelBase
{
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    public DateTime NgayCapNhap { get; set; } = DateTime.UtcNow;
    public bool XoaTam { get; set; } = false;
}