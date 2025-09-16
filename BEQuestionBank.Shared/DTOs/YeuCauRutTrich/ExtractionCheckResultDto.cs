using System.Collections.Generic;

namespace BEQuestionBank.Shared.DTOs
{
    public class ExtractionCheckResultDto
    {
        public int TotalSuccess { get; set; }
        public int TotalFailure { get; set; }
        public Dictionary<int, int> CloExtracted { get; set; } = new Dictionary<int, int>();
        public int SingleQuestions { get; set; }
        public int GroupQuestions { get; set; }
        public string Message { get; set; } = "OK";
        public bool CanExtract { get; set; } = true;
    }
}