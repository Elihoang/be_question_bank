using BEQuestionBank.Domain.Enums;
using Newtonsoft.Json;

public class RutTrichRequest
{
    [JsonProperty("totalQuestions")]
    public int TotalQuestions { get; set; }

    [JsonProperty("cloPerPart")]
    public bool CloPerPart { get; set; }

    [JsonProperty("parts")]
    public List<PartRequest> Parts { get; set; } = new();

    [JsonProperty("clos")]
    public List<CloRequest> Clos { get; set; } = new();
}

public class PartRequest
{
    [JsonProperty("maPhan")]
    public Guid MaPhan { get; set; }

    [JsonProperty("numQuestions")]
    public int NumQuestions { get; set; }

    [JsonProperty("clos")]
    public List<CloRequest> Clos { get; set; } = new();
}

public class CloRequest
{
    [JsonProperty("clo")]
    public int Clo { get; set; }

    [JsonProperty("num")]
    public int Num { get; set; }
}