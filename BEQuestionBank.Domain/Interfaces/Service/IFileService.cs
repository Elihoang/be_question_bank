using BEQuestionBank.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using File = BEQuestionBank.Domain.Models.File;
namespace BEQuestionBank.Domain.Interfaces.Service;

public interface IFileService : IService<File>
{
    Task<FileDto> AddAsync(IFormFile entity, Guid? maCauHoi, Guid? maCauTraLoi);
    Task<IEnumerable<FileDto>> FindFilesByCauHoiOrCauTraLoiAsync(Guid? maCauHoi, Guid? maCauTraLoi);
    
}