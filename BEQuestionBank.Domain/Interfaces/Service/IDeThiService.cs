using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using BEQuestionBank.Shared.DTOs.DeThi;
using Microsoft.AspNetCore.Http;

namespace BEQuestionBank.Domain.Interfaces.Service
{
    public interface IDeThiService : IService<DeThi>
    {
        Task<DeThiDto> GetByIdWithChiTietAsync(Guid id);
        Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync();
        Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto);
        Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto);
        Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc);
        Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync();
        Task<DeThiDto> ImportMaTranFromExcelAsync(Guid maYeuCau,IFormFile excelFile);
        Task<DeThiDto> ManualSelectCauHoiAsync(Guid maYeuCau, List<Guid> maCauHoiList);
        Task<DeThiDto> ChangerStatusAsync(Guid id, bool DaDuyet);
        Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi);
        Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi, ExamTemplateParametersDto parameters);
  
        Task<IEnumerable<CauTraLoiDto>> GetCauTraLoiByDeThiAsync(Guid maDeThi);
        Task<DeThiWithChiTietAndCauTraLoiDto> GetDeThiWithChiTietAndCauTraLoiAsync(Guid maDeThi);
    }
}