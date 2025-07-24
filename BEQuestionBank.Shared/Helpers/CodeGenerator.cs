using NanoidDotNet;

namespace BEQuestionBank.Shared.Helpers;

public static class CodeGenerator
{
    // Độ dài mặc định
    private const int DefaultLength = 8;

    // Độ dài riêng cho từng loại mã
    private const int KhoaCodeLength = 5;     // Mã Khoa ngắn hơn
    private const int DeThiCodeLength = 10;
    private const int FileCodeLength = 10;
    private const int CauHoiCodeLength = 10;
    private const int TraLoiCodeLength = 10;
    private const int PhanCodeLength = 5;

    /// <summary>
    /// Sinh mã chung với tiền tố tùy chọn
    /// </summary>
    public static string Generate(string prefix, int length = DefaultLength)
    {
        string code = Nanoid.Generate(size: length);
        return string.IsNullOrEmpty(prefix) ? code : $"{prefix}_{code}";
    }

    /// <summary>
    /// Sinh mã cho Khoa (ví dụ: KHOA_ab12C)
    /// </summary>
    public static string GenerateKhoaCode() => Generate("FA", KhoaCodeLength);

    /// <summary>
    /// Sinh mã cho Đề Thi (ví dụ: DE_abcD1234)
    /// </summary>
    public static string GenerateDeThiCode() => Generate("EX", DeThiCodeLength);

    /// <summary>
    /// Sinh mã cho File (ví dụ: FILE_abCDefgH12)
    /// </summary>
    public static string GenerateFileCode() => Generate("FI", FileCodeLength);

    /// <summary>
    /// Sinh mã cho Câu Hỏi (ví dụ: CH_abCdEfG12)
    /// </summary>
    public static string GenerateCauHoiCode() => Generate("QE", CauHoiCodeLength);

    /// <summary>
    /// Sinh mã cho Trả Lời (ví dụ: TL_HjKlmN902)
    /// </summary>
    public static string GenerateTraLoiCode() => Generate("AN", TraLoiCodeLength);

    /// <summary>
    /// Sinh mã cho Phần (ví dụ: PHAN_abC12Xy)
    /// </summary>
    public static string GeneratePhanCode() => Generate("CH", PhanCodeLength);
}
