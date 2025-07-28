using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class CauHoiRepository : GenericRepository<CauHoi>, ICauHoiRepository
    {
        private readonly AppDbContext _context;

        public CauHoiRepository(AppDbContext context) 
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CauHoi>> GetAllWithAnswersAsync()
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

        public async Task<CauHoi> GetByIdWithAnswersAsync(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new ArgumentException("ID không đúng định dạng GUID.", nameof(id));
            }

            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .FirstOrDefaultAsync(c => c.MaCauHoi == guidId);
        }
    }
}