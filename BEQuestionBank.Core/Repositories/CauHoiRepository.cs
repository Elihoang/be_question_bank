using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using Microsoft.EntityFrameworkCore.Storage;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class CauHoiRepository : GenericRepository<CauHoi>, ICauHoiRepository
    {
        private readonly AppDbContext _context;
        private readonly ICauTraLoiRepository _cauTraLoiRepository;

        public CauHoiRepository(AppDbContext context, ICauTraLoiRepository cauTraLoiRepository)
            : base(context)
        {
            _context = context;
            _cauTraLoiRepository = cauTraLoiRepository;
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllWithAnswersAsync()
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.XoaTam == false)
                .AsNoTracking()
                .Select(c => new CauHoiDto
                {
                    MaCauHoi = c.MaCauHoi,
                    MaPhan = c.MaPhan,
                    MaSoCauHoi = c.MaSoCauHoi,
                    NoiDung = c.NoiDung,
                    HoanVi = c.HoanVi,
                    CapDo = c.CapDo,
                    SoCauHoiCon = c.SoCauHoiCon,
                    DoPhanCach = c.DoPhanCach,
                    MaCauHoiCha = c.MaCauHoiCha,
                    XoaTam = c.XoaTam,
                    SoLanDuocThi = c.SoLanDuocThi,
                    SoLanDung = c.SoLanDung,
                    NgayTao = c.NgayTao,
                    NgaySua = c.NgaySua,
                    CLO = c.CLO,
                    CauTraLois = c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CauHoiDto> GetByIdWithAnswersAsync(Guid id)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaCauHoi == id && c.XoaTam == false)
                .AsNoTracking()
                .Select(c => new CauHoiDto
                {
                    MaCauHoi = c.MaCauHoi,
                    MaPhan = c.MaPhan,
                    MaSoCauHoi = c.MaSoCauHoi,
                    NoiDung = c.NoiDung,
                    HoanVi = c.HoanVi,
                    CapDo = c.CapDo,
                    SoCauHoiCon = c.SoCauHoiCon,
                    DoPhanCach = c.DoPhanCach,
                    MaCauHoiCha = c.MaCauHoiCha,
                    XoaTam = c.XoaTam,
                    SoLanDuocThi = c.SoLanDuocThi,
                    SoLanDung = c.SoLanDung,
                    NgayTao = c.NgayTao,
                    NgaySua = c.NgaySua,
                    CLO = c.CLO,
                    CauTraLois = c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.CLO == maCLo && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaPhan == maPhan && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.Phan.MaMonHoc == maMonHoc && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaCauHoiCha == maCHCha && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllGroupsAsync()
        {
            Expression<Func<CauHoi, CauHoiDto>> mapCauHoiToDto = c => new CauHoiDto
            {
                MaCauHoi = c.MaCauHoi,
                MaPhan = c.MaPhan,
                MaSoCauHoi = c.MaSoCauHoi,
                NoiDung = c.NoiDung,
                HoanVi = c.HoanVi,
                CapDo = c.CapDo,
                SoCauHoiCon = c.SoCauHoiCon,
                DoPhanCach = c.DoPhanCach,
                MaCauHoiCha = c.MaCauHoiCha,
                XoaTam = c.XoaTam,
                SoLanDuocThi = c.SoLanDuocThi,
                SoLanDung = c.SoLanDung,
                NgayTao = c.NgayTao,
                NgaySua = c.NgaySua,
                CLO = c.CLO,
                CauTraLois = c.CauTraLois != null 
                    ? c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                    : new List<CauTraLoiDto>(),
                CauHoiCons = c.CauHoiCons
                    .Where(con => con.XoaTam == false)
                    .Select(con => new CauHoiDto
                    {
                        MaCauHoi = con.MaCauHoi,
                        MaPhan = con.MaPhan,
                        MaSoCauHoi = con.MaSoCauHoi,
                        NoiDung = con.NoiDung,
                        HoanVi = con.HoanVi,
                        CapDo = con.CapDo,
                        SoCauHoiCon = con.SoCauHoiCon,
                        DoPhanCach = con.DoPhanCach,
                        MaCauHoiCha = con.MaCauHoiCha,
                        XoaTam = con.XoaTam,
                        SoLanDuocThi = con.SoLanDuocThi,
                        SoLanDung = con.SoLanDung,
                        NgayTao = con.NgayTao,
                        NgaySua = con.NgaySua,
                        CLO = con.CLO,
                        CauTraLois = con.CauTraLois != null 
                            ? con.CauTraLois.Select(ctl => new CauTraLoiDto
                            {
                                MaCauTraLoi = ctl.MaCauTraLoi,
                                MaCauHoi = ctl.MaCauHoi,
                                NoiDung = ctl.NoiDung,
                                ThuTu = ctl.ThuTu,
                                LaDapAn = ctl.LaDapAn,
                                HoanVi = ctl.HoanVi
                            }).ToList()
                            : new List<CauTraLoiDto>(),
                        CauHoiCons = new List<CauHoiDto>()
                    }).ToList()
            };

            var result = await _context.CauHois
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                    .ThenInclude(con => con.CauTraLois)
                .Where(c => c.CauHoiCons.Any() && c.XoaTam == false)
                .AsNoTracking()
                .Select(mapCauHoiToDto)
                .ToListAsync();

            return result ?? new List<CauHoiDto>();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<CauHoiDto> AddWithAnswersAsync(CreateCauHoiWithAnswersDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                throw new ArgumentNullException(nameof(cauHoiDto), "Dữ liệu câu hỏi không được null.");
            }

            if (string.IsNullOrWhiteSpace(cauHoiDto.NoiDung))
            {
                throw new ArgumentException("Nội dung câu hỏi không được để trống.", nameof(cauHoiDto.NoiDung));
            }

            if (cauHoiDto.MaCauHoiCha.HasValue)
            {
                var parentExists = await _context.CauHois.AnyAsync(c => c.MaCauHoi == cauHoiDto.MaCauHoiCha.Value);
                if (!parentExists)
                {
                    throw new ArgumentException($"Câu hỏi cha với mã {cauHoiDto.MaCauHoiCha} không tồn tại.");
                }
            }

            if (cauHoiDto.CauTraLois.Any())
            {
                var correctAnswersCount = cauHoiDto.CauTraLois.Count(a => a.LaDapAn);
                if (correctAnswersCount > 1)
                {
                    throw new ArgumentException("Chỉ được phép có một câu trả lời đúng.");
                }
            }

            // Không mở transaction ở đây
            var cauHoi = new CauHoi
            {
                MaCauHoi = Guid.NewGuid(),
                MaPhan = cauHoiDto.MaPhan,
                MaSoCauHoi = cauHoiDto.MaSoCauHoi,
                NoiDung = cauHoiDto.NoiDung,
                HoanVi = cauHoiDto.HoanVi,
                CapDo = cauHoiDto.CapDo,
                SoCauHoiCon = cauHoiDto.SoCauHoiCon,
                DoPhanCach = cauHoiDto.DoPhanCach,
                MaCauHoiCha = cauHoiDto.MaCauHoiCha,
                XoaTam = false,
                SoLanDuocThi = null,
                SoLanDung = null,
                CLO = cauHoiDto.CLO,
                NgayTao = DateTime.UtcNow,
                NgaySua = null
            };

            await _dbSet.AddAsync(cauHoi);
            await _context.SaveChangesAsync();

            foreach (var answerDto in cauHoiDto.CauTraLois)
            {
                var cauTraLoi = new CauTraLoi
                {
                    MaCauTraLoi = Guid.NewGuid(),
                    MaCauHoi = cauHoi.MaCauHoi,
                    NoiDung = answerDto.NoiDung,
                    ThuTu = answerDto.ThuTu,
                    LaDapAn = answerDto.LaDapAn,
                    HoanVi = answerDto.HoanVi
                };

                await _cauTraLoiRepository.AddAsync(cauTraLoi);
            }

            var result = new CauHoiDto
            {
                MaCauHoi = cauHoi.MaCauHoi,
                MaPhan = cauHoi.MaPhan,
                MaSoCauHoi = cauHoi.MaSoCauHoi,
                NoiDung = cauHoi.NoiDung,
                HoanVi = cauHoi.HoanVi,
                CapDo = cauHoi.CapDo,
                SoCauHoiCon = cauHoi.SoCauHoiCon,
                DoPhanCach = cauHoi.DoPhanCach,
                MaCauHoiCha = cauHoi.MaCauHoiCha,
                XoaTam = cauHoi.XoaTam,
                SoLanDuocThi = cauHoi.SoLanDuocThi,
                SoLanDung = cauHoi.SoLanDung,
                NgayTao = cauHoi.NgayTao,
                NgaySua = cauHoi.NgaySua,
                CLO = cauHoi.CLO,
                CauTraLois = cauHoiDto.CauTraLois.Select(a => new CauTraLoiDto
                {
                    MaCauTraLoi = Guid.NewGuid(), // hoặc lấy từ DB nếu cần
                    MaCauHoi = cauHoi.MaCauHoi,
                    NoiDung = a.NoiDung,
                    ThuTu = a.ThuTu,
                    LaDapAn = a.LaDapAn,
                    HoanVi = a.HoanVi
                }).ToList(),
                CauHoiCons = new List<CauHoiDto>()
            };

            return result;
        }
        public async Task<CauHoiDto> UpdateWithAnswersAsync(Guid maCauHoi, UpdateCauHoiWithAnswersDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                throw new ArgumentNullException(nameof(cauHoiDto), "Dữ liệu câu hỏi không được null.");
            }

            var existingCauHoi = await _context.CauHois
                .Include(c => c.CauTraLois)
                .FirstOrDefaultAsync(c => c.MaCauHoi == maCauHoi && c.XoaTam == false);

            if (existingCauHoi == null)
            {
                throw new ArgumentException($"Không tìm thấy câu hỏi với mã {maCauHoi}.");
            }

            if (!string.IsNullOrWhiteSpace(cauHoiDto.NoiDung) && cauHoiDto.NoiDung != existingCauHoi.NoiDung)
            {
                if (string.IsNullOrWhiteSpace(cauHoiDto.NoiDung))
                {
                    throw new ArgumentException("Nội dung câu hỏi không được để trống.", nameof(cauHoiDto.NoiDung));
                }
                existingCauHoi.NoiDung = cauHoiDto.NoiDung;
            }

            if (cauHoiDto.MaCauHoiCha.HasValue && cauHoiDto.MaCauHoiCha != existingCauHoi.MaCauHoiCha)
            {
                var parentExists = await _context.CauHois.AnyAsync(c => c.MaCauHoi == cauHoiDto.MaCauHoiCha.Value);
                if (!parentExists)
                {
                    throw new ArgumentException($"Câu hỏi cha với mã {cauHoiDto.MaCauHoiCha} không tồn tại.");
                }
                existingCauHoi.MaCauHoiCha = cauHoiDto.MaCauHoiCha;
            }

            if (cauHoiDto.CauTraLois.Any())
            {
                var correctAnswersCount = cauHoiDto.CauTraLois.Count(a => a.LaDapAn);
                if (correctAnswersCount > 1)
                {
                    throw new ArgumentException("Chỉ được phép có một câu trả lời đúng.");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật thuộc tính câu hỏi nếu được cung cấp
                if (cauHoiDto.MaPhan != Guid.Empty) existingCauHoi.MaPhan = cauHoiDto.MaPhan;
                if (cauHoiDto.MaSoCauHoi != 0) existingCauHoi.MaSoCauHoi = cauHoiDto.MaSoCauHoi;
                if (cauHoiDto.HoanVi.HasValue) existingCauHoi.HoanVi = cauHoiDto.HoanVi.Value;
                if (cauHoiDto.CapDo.HasValue) existingCauHoi.CapDo = cauHoiDto.CapDo.Value;
                if (cauHoiDto.SoCauHoiCon.HasValue) existingCauHoi.SoCauHoiCon = cauHoiDto.SoCauHoiCon.Value;
                if (cauHoiDto.DoPhanCach != 0) existingCauHoi.DoPhanCach = cauHoiDto.DoPhanCach;
                if (cauHoiDto.XoaTam.HasValue) existingCauHoi.XoaTam = cauHoiDto.XoaTam.Value;
                if (cauHoiDto.SoLanDuocThi.HasValue) existingCauHoi.SoLanDuocThi = cauHoiDto.SoLanDuocThi.Value;
                if (cauHoiDto.SoLanDung.HasValue) existingCauHoi.SoLanDung = cauHoiDto.SoLanDung.Value;
                if (cauHoiDto.CLO.HasValue) existingCauHoi.CLO = cauHoiDto.CLO.Value;
                existingCauHoi.NgaySua = DateTime.UtcNow;

                _context.CauHois.Update(existingCauHoi);
                await _context.SaveChangesAsync();

                // Xử lý câu trả lời
                var existingAnswerIds = existingCauHoi.CauTraLois.Select(a => a.MaCauTraLoi).ToList();
                var newAnswerIds = cauHoiDto.CauTraLois.Where(a => a.MaCauTraLoi.HasValue).Select(a => a.MaCauTraLoi.Value).ToList();

                // Xóa các câu trả lời không còn trong danh sách mới
                var answersToDelete = existingCauHoi.CauTraLois.Where(a => !newAnswerIds.Contains(a.MaCauTraLoi)).ToList();
                foreach (var answer in answersToDelete)
                {
                    await _cauTraLoiRepository.DeleteAsync(answer);
                }

                // Thêm hoặc cập nhật câu trả lời
                foreach (var answerDto in cauHoiDto.CauTraLois)
                {
                    if (answerDto.MaCauTraLoi.HasValue && existingAnswerIds.Contains(answerDto.MaCauTraLoi.Value))
                    {
                        var existingAnswer = existingCauHoi.CauTraLois.First(a => a.MaCauTraLoi == answerDto.MaCauTraLoi.Value);
                        existingAnswer.NoiDung = answerDto.NoiDung; 
                        existingAnswer.ThuTu = answerDto.ThuTu;     
                        existingAnswer.LaDapAn = answerDto.LaDapAn; 
                        existingAnswer.HoanVi = answerDto.HoanVi;  
                        await _cauTraLoiRepository.UpdateAsync(existingAnswer);
                    }
                    else
                    {
                        // Thêm câu trả lời mới
                        var newAnswer = new CauTraLoi
                        {
                            MaCauTraLoi = Guid.NewGuid(),
                            MaCauHoi = maCauHoi,
                            NoiDung = answerDto.NoiDung,
                            ThuTu = answerDto.ThuTu,
                            LaDapAn = answerDto.LaDapAn,
                            HoanVi = answerDto.HoanVi
                        };
                        await _cauTraLoiRepository.AddAsync(newAnswer);
                    }
                }

                await transaction.CommitAsync();

                // Trả về DTO
                var updatedCauHoi = await _context.CauHois
                    .Include(c => c.CauTraLois)
                    .Where(c => c.MaCauHoi == maCauHoi)
                    .Select(c => new CauHoiDto
                    {
                        MaCauHoi = c.MaCauHoi,
                        MaPhan = c.MaPhan,
                        MaSoCauHoi = c.MaSoCauHoi,
                        NoiDung = c.NoiDung,
                        HoanVi = c.HoanVi,
                        CapDo = c.CapDo,
                        SoCauHoiCon = c.SoCauHoiCon,
                        DoPhanCach = c.DoPhanCach,
                        MaCauHoiCha = c.MaCauHoiCha,
                        XoaTam = c.XoaTam,
                        SoLanDuocThi = c.SoLanDuocThi,
                        SoLanDung = c.SoLanDung,
                        NgayTao = c.NgayTao,
                        NgaySua = c.NgaySua,
                        CLO = c.CLO,
                        CauTraLois = c.CauTraLois.Select(ctl => new CauTraLoiDto
                        {
                            MaCauTraLoi = ctl.MaCauTraLoi,
                            MaCauHoi = ctl.MaCauHoi,
                            NoiDung = ctl.NoiDung,
                            ThuTu = ctl.ThuTu,
                            LaDapAn = ctl.LaDapAn,
                            HoanVi = ctl.HoanVi
                        }).ToList(),
                        CauHoiCons = new List<CauHoiDto>()
                    })
                    .FirstOrDefaultAsync();

                return updatedCauHoi;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
    }
}
