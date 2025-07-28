using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Application.Services;

    public class CauTraLoiRepository(AppDbContext context) : GenericRepository<CauTraLoi>(context), ICauTraLoiRepository
    {
        private readonly AppDbContext _context = context;

    public async Task<IEnumerable<CauTraLoi>> GetByMaCauHoi(Guid maCauHoi)
    {
        return await _context.CauTraLois.Where( cTL => cTL.MaCauHoi == maCauHoi).ToListAsync();
    }
}
