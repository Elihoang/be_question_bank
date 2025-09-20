using System;

namespace BEQuestionBank.Shared.DTOs
{
    public class ExtractionCheckDto
    {
        public Guid MaMonHoc { get; set; }
        public string MaTran { get; set; }  // JSON string của RutTrichRequest
    }
}