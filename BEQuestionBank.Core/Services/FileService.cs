using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using File = BEQuestionBank.Domain.Models.File;
using IOFile = System.IO.File; // Bí danh cho System.IO.File

namespace BEQuestionBank.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly string _storagePath; // Ví dụ: "wwwroot/media"
        private readonly ILogger<FileService> _logger;

        public FileService(IFileRepository fileRepository, string storagePath, ILogger<FileService> logger)
        {
            _fileRepository = fileRepository;
            _storagePath = storagePath;
            _logger = logger;
        }

        public async Task<FileDto> AddAsync(IFormFile formFile, Guid? maCauHoi, Guid? maCauTraLoi)
        {
            if (formFile == null || formFile.Length == 0)
            {
                _logger.LogError("Tệp tải lên không hợp lệ.");
                throw new ArgumentException("Tệp không hợp lệ.");
            }

            var extension = Path.GetExtension(formFile.FileName).ToLower();
    
            FileType loaiFile = extension switch
            {
                ".jpg" or ".png" or ".jpeg" => FileType.Image,
                ".mp3" => FileType.Audio,
                _ => throw new ArgumentException($"Định dạng tệp {extension} không được hỗ trợ.")
            };

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_storagePath, fileName);

            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(_storagePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var file = new File
            {
                MaFile = Guid.NewGuid(),
                MaCauHoi = maCauHoi,
                MaCauTraLoi = maCauTraLoi,
                TenFile = filePath,
                LoaiFile = loaiFile
            };

            await _fileRepository.AddAsync(file);

            _logger.LogInformation("Lưu tệp {TenFile} thành công cho MaCauHoi: {MaCauHoi}, MaCauTraLoi: {MaCauTraLoi}", fileName, maCauHoi, maCauTraLoi);

            return new FileDto
            {
                MaFile = file.MaFile,
                MaCauHoi = file.MaCauHoi,
                MaCauTraLoi = file.MaCauTraLoi,
                TenFile = file.TenFile,
                LoaiFile = file.LoaiFile
            };
        }


        public async Task<File> GetByIdAsync(Guid maFile)
        {
            var file = await _fileRepository.GetByIdAsync(maFile);
            if (file == null)
            {
                _logger.LogError("Không tìm thấy tệp với mã {MaFile}.", maFile);
                throw new ArgumentException($"Không tìm thấy tệp với mã {maFile}.");
            }
            return file;
        }

        public async Task<IEnumerable<File>> GetAllAsync()
        {
            var files = await _fileRepository.GetAllAsync();
            _logger.LogInformation("Lấy danh sách tất cả tệp thành công.");
            return files;
        }

        public async Task<IEnumerable<File>> FindAsync(Expression<Func<File, bool>> predicate)
        {
            var files = await _fileRepository.FindAsync(predicate);
            _logger.LogInformation("Tìm kiếm tệp theo điều kiện thành công.");
            return files;
        }

        public async Task<File> FirstOrDefaultAsync(Expression<Func<File, bool>> predicate)
        {
            var file = await _fileRepository.FirstOrDefaultAsync(predicate);
            if (file == null)
            {
                _logger.LogWarning("Không tìm thấy tệp theo điều kiện.");
            }
            return file;
        }

        public async Task AddAsync(File entity)
        {
            if (entity == null)
            {
                _logger.LogError("Dữ liệu tệp không được null.");
                throw new ArgumentNullException(nameof(entity));
            }
            await _fileRepository.AddAsync(entity);
            _logger.LogInformation("Thêm tệp {MaFile} thành công.", entity.MaFile);
        }

        public async Task UpdateAsync(File entity)
        {
            if (entity == null)
            {
                _logger.LogError("Dữ liệu tệp không được null.");
                throw new ArgumentNullException(nameof(entity));
            }
            await _fileRepository.UpdateAsync(entity);
            _logger.LogInformation("Cập nhật tệp {MaFile} thành công.", entity.MaFile);
        }

        public async Task DeleteAsync(File entity)
        {
            if (entity == null)
            {
                _logger.LogError("Dữ liệu tệp không được null.");
                throw new ArgumentNullException(nameof(entity));
            }
            if (IOFile.Exists(entity.TenFile))
            {
                IOFile.Delete(entity.TenFile);
                _logger.LogInformation("Xóa tệp vật lý {TenFile} thành công.", entity.TenFile);
            }
            await _fileRepository.DeleteAsync(entity);
            _logger.LogInformation("Xóa thông tin tệp {MaFile} thành công.", entity.MaFile);
        }

        public async Task<bool> ExistsAsync(Expression<Func<File, bool>> predicate)
        {
            return await _fileRepository.ExistsAsync(predicate);
        }
    }
}