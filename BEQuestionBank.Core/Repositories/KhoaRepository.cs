using System.Linq.Expressions;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Core.Repositories;

public class KhoaRepository : GenericRepository<Khoa>, IKhoaRepository
{
    public KhoaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Khoa> GetByIdAsync(string id)
    {
        return await _context.Khoas.FindAsync(id);
    }

    public async Task<IEnumerable<Khoa>> GetAllAsync()
    {
        return await _context.Khoas.ToListAsync();
    }

    public async Task<IEnumerable<Khoa>> FindAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _context.Khoas.Where(predicate).ToListAsync();
    }

    public async Task<Khoa> FirstOrDefaultAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _context.Khoas.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Khoa, bool>> predicate)
    {
        return await _context.Khoas.AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<Khoa, bool>> predicate = null)
    {
        return predicate == null 
            ? await _context.Khoas.CountAsync() 
            : await _context.Khoas.CountAsync(predicate);
    }
}