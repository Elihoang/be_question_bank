using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Core.Repositories;

public class MonHocRepository(AppDbContext context) : GenericRepository<MonHoc>(context), IMonHocRepository
{
    private new readonly AppDbContext _context = context;
    public async Task<IEnumerable<MonHoc>> GetByMaKhoaAsync(Guid maKhoa)
    {
        return await _context.MonHocs
            .Where(mH => mH.MaKhoa == maKhoa)
            .ToListAsync();
    }

}