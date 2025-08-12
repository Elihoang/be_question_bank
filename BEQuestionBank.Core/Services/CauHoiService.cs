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
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ICauTraLoiRepository _cauTraLoiRepository;

        public CauHoiService(ICauHoiRepository cauHoiRepository, ICauTraLoiRepository cauTraLoiRepository)
        {
            _cauHoiRepository = cauHoiRepository ?? throw new ArgumentNullException(nameof(cauHoiRepository));
            _cauTraLoiRepository = cauTraLoiRepository ?? throw new ArgumentNullException(nameof(cauTraLoiRepository));
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllWithAnswersAsync()
        {
            return await _cauHoiRepository.GetAllWithAnswersAsync();
        }

        public async Task<CauHoiDto> GetByIdWithAnswersAsync(Guid id)
        {
            return await _cauHoiRepository.GetByIdWithAnswersAsync(id);
        }

        public async Task<CauHoi> GetByIdAsync(Guid id)
        {
            return await _cauHoiRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<CauHoi>> GetAllAsync()
        {
            return _cauHoiRepository.GetAllAsync();
        }

        public Task<IEnumerable<CauHoi>> FindAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _cauHoiRepository.FindAsync(predicate);
        }

        public Task<CauHoi> FirstOrDefaultAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _cauHoiRepository.FirstOrDefaultAsync(predicate);
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

            return _cauHoiRepository.AddAsync(model);
        }

        public async Task<CauHoiDto> AddWithAnswersAsync(CreateCauHoiWithAnswersDto cauHoiDto)
        {
            return await _cauHoiRepository.AddWithAnswersAsync(cauHoiDto);
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

            return _cauHoiRepository.UpdateAsync(model);
        }

        public Task DeleteAsync(CauHoi model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Câu hỏi không được null.");
            }

            return _cauHoiRepository.DeleteAsync(model);
        }

        public Task<bool> ExistsAsync(Expression<Func<CauHoi, bool>> predicate)
        {
            return _cauHoiRepository.ExistsAsync(predicate);
        }

        public Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo)
        {
            return _cauHoiRepository.GetByCLoAsync(maCLo);
        }

        public Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha)
        {
            return _cauHoiRepository.GetByMaCauHoiChasync(maCHCha);
        }

        public Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan)
        {
            return _cauHoiRepository.GetByMaPhanAsync(maPhan);
        }

        public Task<IEnumerable<CauHoiDto>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return _cauHoiRepository.GetByMaMonHocAsync(maMonHoc);
        }

        public Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi)
        {
            return _cauHoiRepository.GetByMaDeThiAsync(maDeThi);
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllGroupsAsync()
        {
            var result = await _cauHoiRepository.GetAllGroupsAsync();
            return result ?? new List<CauHoiDto>();
        }
        public async Task<CauHoiDto> UpdateWithAnswersAsync(Guid maCauHoi, UpdateCauHoiWithAnswersDto cauHoiDto)
        {
            return await _cauHoiRepository.UpdateWithAnswersAsync(maCauHoi, cauHoiDto);
        }

    }
}