using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;

namespace BEQuestionBank.Core.Services
{
    public class YeuCauRutTrichService : IYeuCauRutTrichService
    {
        private readonly IYeuCauRutTrichRepository _repository;

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

        public async Task AddAsync(YeuCauRutTrich entity)
        {
            if (!await _repository.ExistsNguoiDungAsync(entity.MaNguoiDung))
            {
                throw new Exception($"Mã người dùng {entity.MaNguoiDung} không tồn tại.");
            }

            if (!await _repository.ExistsMonHocAsync(entity.MaMonHoc))
            {
                throw new Exception($"Mã môn học {entity.MaMonHoc} không tồn tại.");
            }

            entity.MaYeuCau = Guid.NewGuid();
            entity.NgayYeuCau = DateTime.UtcNow;
            await _repository.AddAsync(entity);
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
                DaXuLy = e.DaXuLy
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
                DaXuLy = e.DaXuLy
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
                DaXuLy = e.DaXuLy
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
                DaXuLy = entity.DaXuLy
            };
        }
    }
}