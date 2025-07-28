using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Core.Repositories;

public class PhanRepository(AppDbContext context) : GenericRepository<Phan>(context), IPhanRepository
{
    private new readonly AppDbContext _context = context;
    
    public async Task<IEnumerable<Phan>> GetByMaMonHocAsync(Guid maMonHoc)
    {
        return await _context.Phans
            .Where(p => p.MaMonHoc == maMonHoc)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task AddAsync(Phan entity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var countPhan = await _context.Phans
                .Where(p => p.MaMonHoc == entity.MaMonHoc).CountAsync();

            entity.ThuTu = countPhan + 1;
            
            await base.AddAsync(entity);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}