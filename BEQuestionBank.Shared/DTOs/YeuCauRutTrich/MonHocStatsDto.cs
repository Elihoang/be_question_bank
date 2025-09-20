namespace BEQuestionBank.Shared.DTOs;

public class MonHocStatsDto
{
    public List<MatrixDto> Matrix { get; set; } = new List<MatrixDto>();
    public Dictionary<string, int> CloTotals { get; set; } = new Dictionary<string, int>();  // CLO1: count, etc.
    public int TotalQuestions { get; set; }
    public int SingleQuestions { get; set; }
    public int GroupQuestions { get; set; }
    public bool CloPerPart { get; set; }
}