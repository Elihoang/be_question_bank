using System.Text.Json.Serialization;

public class MatrixDto
{
    public string Chapter { get; set; }
    public Guid? MaPhan { get; set; }

    [JsonPropertyName("CLO1")]
    public int CLO1 { get; set; }

    [JsonPropertyName("CLO2")]
    public int CLO2 { get; set; }

    [JsonPropertyName("CLO3")]
    public int CLO3 { get; set; }

    [JsonPropertyName("CLO4")]
    public int CLO4 { get; set; }

    [JsonPropertyName("CLO5")]
    public int CLO5 { get; set; }

    public int Total { get; set; }
}