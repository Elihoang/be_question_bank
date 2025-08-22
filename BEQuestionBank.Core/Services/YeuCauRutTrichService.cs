using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using Newtonsoft.Json;
using Serilog;

namespace BEQuestionBank.Core.Services
{
    public class YeuCauRutTrichService : IYeuCauRutTrichService
    {
        private readonly IYeuCauRutTrichRepository _repository;
        private IYeuCauRutTrichService _yeuCauRutTrichServiceImplementation;

        public YeuCauRutTrichService(IYeuCauRutTrichRepository repository)
        {
            _repository = repository;
        }

        public async Task<YeuCauRutTrich> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy yêu cầu rút trích với mã {id}.");
            }
            return entity;
        }

        public async Task<IEnumerable<YeuCauRutTrich>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<YeuCauRutTrich>> FindAsync(Expression<Func<YeuCauRutTrich, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public async Task<YeuCauRutTrich> FirstOrDefaultAsync(Expression<Func<YeuCauRutTrich, bool>> predicate)
        {
            var entity = await _repository.FirstOrDefaultAsync(predicate);
            if (entity == null)
            {
                throw new Exception("Không tìm thấy yêu cầu rút trích phù hợp.");
            }
            return entity;
        }

        Task IService<YeuCauRutTrich>.AddAsync(YeuCauRutTrich entity)
        {
            return AddAsync(entity);
        }

        public async Task<YeuCauRutTrich> AddAsync(YeuCauRutTrich entity)
        {
            if (!await _repository.ExistsNguoiDungAsync(entity.MaNguoiDung))
                throw new Exception($"Mã người dùng {entity.MaNguoiDung} không tồn tại.");

            if (!await _repository.ExistsMonHocAsync(entity.MaMonHoc))
                throw new Exception($"Mã môn học {entity.MaMonHoc} không tồn tại.");

            // Validate JSON trong MaTran
            if (!string.IsNullOrWhiteSpace(entity.MaTran))
            {
                try
                {
                    var request = JsonConvert.DeserializeObject<RutTrichRequest>(entity.MaTran);
                    if (request.TotalQuestions <= 0)
                        throw new Exception("TotalQuestions phải lớn hơn 0.");

                    if (request.CloPerPart)
                    {
                        if (!request.Parts.Any())
                            throw new Exception("Parts không được rỗng khi cloPerPart = true.");

                        int total = request.Parts.Sum(p => p.NumQuestions);
                        if (total != request.TotalQuestions)
                            throw new Exception($"Tổng numQuestions từ parts ({total}) không khớp với totalQuestions ({request.TotalQuestions}).");

                        foreach (var part in request.Parts)
                        {
                            if (!await _repository.ExistsPhanAsync(part.MaPhan))
                                throw new Exception($"Mã phần {part.MaPhan} không tồn tại.");
                            if (part.NumQuestions <= 0)
                                throw new Exception($"NumQuestions của phần {part.MaPhan} phải lớn hơn 0.");
                            if (part.Clos.Sum(c => c.Num) != part.NumQuestions)
                                throw new Exception($"Tổng CLO num của phần {part.MaPhan} không khớp với numQuestions.");
                            foreach (var clo in part.Clos)
                            {
                                if (!Enum.IsDefined(typeof(EnumCLO), clo.Clo))
                                    throw new Exception($"CLO {clo.Clo} không hợp lệ.");
                            }
                        }
                    }
                    else
                    {
                        if (!request.Clos.Any())
                            throw new Exception("Clos không được rỗng khi cloPerPart = false.");
                        if (request.Clos.Sum(c => c.Num) != request.TotalQuestions)
                            throw new Exception("Tổng CLO num không khớp với totalQuestions.");
                        foreach (var clo in request.Clos)
                        {
                            if (!Enum.IsDefined(typeof(EnumCLO), clo.Clo))
                                throw new Exception($"CLO {clo.Clo} không hợp lệ.");
                        }
                    }
                }
                catch (JsonException)
                {
                    throw new Exception("MaTran không phải JSON hợp lệ.");
                }
            }

            entity.MaYeuCau = Guid.NewGuid();
            entity.NgayYeuCau = DateTime.UtcNow;
            entity.DaXuLy = false;
            await _repository.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(YeuCauRutTrich entity)
        {
            if (!await _repository.ExistsAsync(y => y.MaYeuCau == entity.MaYeuCau))
            {
                throw new Exception($"Không tìm thấy yêu cầu rút trích với mã {entity.MaYeuCau}.");
            }

            if (!await _repository.ExistsNguoiDungAsync(entity.MaNguoiDung))
            {
                throw new Exception($"Mã người dùng {entity.MaNguoiDung} không tồn tại.");
            }

            if (!await _repository.ExistsMonHocAsync(entity.MaMonHoc))
            {
                throw new Exception($"Mã môn học {entity.MaMonHoc} không tồn tại.");
            }

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(YeuCauRutTrich entity)
        {
            if (!await _repository.ExistsAsync(y => y.MaYeuCau == entity.MaYeuCau))
            {
                throw new Exception($"Không tìm thấy yêu cầu rút trích với mã {entity.MaYeuCau}.");
            }

            await _repository.DeleteAsync(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<YeuCauRutTrich, bool>> predicate)
        {
            return await _repository.ExistsAsync(predicate);
        }

        public async Task<IEnumerable<YeuCauRutTrichDto>> GetByMaNguoiDungAsync(Guid maNguoiDung)
        {
            if (!await _repository.ExistsNguoiDungAsync(maNguoiDung))
            {
                throw new Exception($"Mã người dùng {maNguoiDung} không tồn tại.");
            }

            var entities = await _repository.GetByMaNguoiDungAsync(maNguoiDung);
            return entities.Select(e => new YeuCauRutTrichDto
            {
                MaYeuCau = e.MaYeuCau,
                MaNguoiDung = e.MaNguoiDung,
                MaMonHoc = e.MaMonHoc,
                NoiDungRutTrich = e.NoiDungRutTrich,
                GhiChu = e.GhiChu,
                NgayYeuCau = e.NgayYeuCau,
                NgayXuLy = e.NgayXuLy,
                DaXuLy = e.DaXuLy,
                MaTran = e.MaTran
            });
        }

        public async Task<IEnumerable<YeuCauRutTrichDto>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            if (!await _repository.ExistsMonHocAsync(maMonHoc))
            {
                throw new Exception($"Mã môn học {maMonHoc} không tồn tại.");
            }

            var entities = await _repository.GetByMaMonHocAsync(maMonHoc);
            return entities.Select(e => new YeuCauRutTrichDto
            {
                MaYeuCau = e.MaYeuCau,
                MaNguoiDung = e.MaNguoiDung,
                MaMonHoc = e.MaMonHoc,
                NoiDungRutTrich = e.NoiDungRutTrich,
                GhiChu = e.GhiChu,
                NgayYeuCau = e.NgayYeuCau,
                NgayXuLy = e.NgayXuLy,
                DaXuLy = e.DaXuLy,
                MaTran = e.MaTran
            });
        }

        public async Task<IEnumerable<YeuCauRutTrichDto>> GetChuaXuLyAsync()
        {
            var entities = await _repository.GetChuaXuLyAsync();
            return entities.Select(e => new YeuCauRutTrichDto
            {
                MaYeuCau = e.MaYeuCau,
                MaNguoiDung = e.MaNguoiDung,
                MaMonHoc = e.MaMonHoc,
                NoiDungRutTrich = e.NoiDungRutTrich,
                GhiChu = e.GhiChu,
                NgayYeuCau = e.NgayYeuCau,
                NgayXuLy = e.NgayXuLy,
                DaXuLy = e.DaXuLy,
                MaTran = e.MaTran
            });
        }

        public async Task<YeuCauRutTrichDto> ChangerStatusAsync(Guid id, bool daXuLy)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy yêu cầu rút trích với mã {id}.");
            }

            entity.DaXuLy = daXuLy;
            entity.NgayXuLy = daXuLy ? DateTime.UtcNow : null;

            await _repository.UpdateAsync(entity);

            return new YeuCauRutTrichDto
            {
                MaYeuCau = entity.MaYeuCau,
                MaNguoiDung = entity.MaNguoiDung,
                MaMonHoc = entity.MaMonHoc,
                NoiDungRutTrich = entity.NoiDungRutTrich,
                GhiChu = entity.GhiChu,
                NgayYeuCau = entity.NgayYeuCau,
                NgayXuLy = entity.NgayXuLy,
                DaXuLy = entity.DaXuLy,
                MaTran = entity.MaTran
            };
        }
    }
}