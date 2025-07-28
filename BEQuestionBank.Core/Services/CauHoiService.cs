using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;

namespace BEQuestionBank.Application.Services
{
    public class CauHoiService : ICauHoiService
    {
        private readonly ICauHoiRepository _repository;

        public CauHoiService(ICauHoiRepository cauHoiRepository)
        {
            _repository = cauHoiRepository;
        }
        public async Task<CauHoi> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public Task<IEnumerable<CauHoi>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<IEnumerable<CauHoi>> FindAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _repository.FindAsync(predicate);
        }

        public Task<CauHoi> FirstOrDefaultAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _repository.FirstOrDefaultAsync(predicate);
        }

        public Task AddAsync(CauHoi model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Câu hỏi không được null.");
            }

            if (string.IsNullOrWhiteSpace(model.NoiDung))
            {
                throw new ArgumentException("Nội dung câu hỏi không được để trống.", nameof(model.NoiDung));
            }

            return _repository.AddAsync(model);
        }

        public Task UpdateAsync(CauHoi model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Câu hỏi không được null.");
            }

            if (string.IsNullOrWhiteSpace(model.NoiDung))
            {
                throw new ArgumentException("Nội dung câu hỏi không được để trống.", nameof(model.NoiDung));
            }

            return _repository.UpdateAsync(model);
        }

        public Task DeleteAsync(CauHoi model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Câu hỏi không được null.");
            }

            return _repository.DeleteAsync(model);
        }

        public Task<bool> ExistsAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _repository.ExistsAsync(predicate);
        }

        public Task<IEnumerable<object>> GetAllWithAnswersAsync()
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo)
        {
            return _repository.GetByCLoAsync(maCLo);
        }

        public Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha)
        {
           return _repository.GetByMaCauHoiChasync(maCHCha);
        }
        public Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan)
        {
            return _repository.GetByMaPhanAsync(maPhan);
        }
        public Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return _repository.GetByMaMonHocAsync(maMonHoc);
        }
        public Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi)
        {
            return _repository.GetByMaDeThiAsync(maDeThi);
        }
    }
}