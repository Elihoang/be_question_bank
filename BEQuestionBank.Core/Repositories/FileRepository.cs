using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Shared.DTOs.File;
using Microsoft.EntityFrameworkCore;
using File = BEQuestionBank.Domain.Models.File;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FileDto>> FindFilesByCauHoiOrCauTraLoiAsync(Guid? maCauHoi, Guid? maCauTraLoi)
        {
            if (maCauHoi == Guid.Empty || maCauTraLoi == Guid.Empty)
            {
                throw new ArgumentException("Mã câu hỏi hoặc mã câu trả lời không hợp lệ.");
            }

            var query = _context.Files.AsQueryable();

            if (maCauHoi.HasValue)
            {
                query = query.Where(f => f.MaCauHoi == maCauHoi);
            }

            if (maCauTraLoi.HasValue)
            {
                query = query.Where(f => f.MaCauTraLoi == maCauTraLoi);
            }

            var files = await query
                .Select(f => new FileDto
                {
                    MaFile = f.MaFile,
                    MaCauHoi = f.MaCauHoi,
                    MaCauTraLoi = f.MaCauTraLoi,
                    TenFile = f.TenFile,
                    LoaiFile = f.LoaiFile
                })
                .ToListAsync();

            return files;
        }

    }
}