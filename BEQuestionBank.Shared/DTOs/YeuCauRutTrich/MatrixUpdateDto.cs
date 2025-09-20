namespace BEQuestionBank.Shared.DTOs;

public class MatrixUpdateDto // Input update từ FE (chỉ CLO values)
{
    public Guid? MaPhan { get; set; }
    public int CLO1 { get; set; }
    public int CLO2 { get; set; }
    public int CLO3 { get; set; }
    public int CLO4 { get; set; }
    public int CLO5 { get; set; }
}