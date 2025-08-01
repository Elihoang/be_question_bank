using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;

namespace BEQuestionBank.Core.Services
{
    public class DeThiService : IDeThiService
    {
        private readonly IDeThiRepository _deThiRepository;

        public DeThiService(IDeThiRepository deThiRepository)
        {
            _deThiRepository = deThiRepository;
        }
        
        public Task<DeThi> GetByIdAsync(Guid id)
        {
            return _deThiRepository.GetByIdAsync(id);
        }
        
        public Task<IEnumerable<DeThi>> GetAllAsync()
        {
            return _deThiRepository.GetAllAsync();
        }

        public Task<IEnumerable<DeThi>> FindAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FindAsync(predicate);
        }

        public Task<DeThi> FirstOrDefaultAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FirstOrDefaultAsync(predicate);
        }

        public Task AddAsync(DeThi entity)
        {
            return _deThiRepository.AddAsync(entity);
        }

        public Task UpdateAsync(DeThi entity)
        {
            return _deThiRepository.UpdateAsync(entity);
        }

        public Task DeleteAsync(DeThi entity)
        {
            return _deThiRepository.DeleteAsync(entity);
        }

        public Task<bool> ExistsAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.ExistsAsync(predicate);
        }

        public Task<DeThiDto> GetByIdWithChiTietAsync(Guid id)
        {
            return _deThiRepository.GetByIdWithChiTietAsync(id);
        }

        public Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync()
        {
            return _deThiRepository.GetAllWithChiTietAsync();
        }

        public Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.AddWithChiTietAsync(deThiDto);
        }

        public Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.UpdateWithChiTietAsync(deThiDto);
        }

        public async Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _deThiRepository.GetByMaMonHocAsync(maMonHoc);
        }

        public async Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync()
        {
            return await _deThiRepository.GetApprovedDeThisAsync();
        }
    }
}