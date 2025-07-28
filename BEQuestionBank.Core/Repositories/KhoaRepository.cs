using System.Linq.Expressions;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BEQuestionBank.Core.Repositories;

public class KhoaRepository(AppDbContext context) : GenericRepository<Khoa>(context), IKhoaRepository
{
    private new readonly AppDbContext _context = context;
    public async Task<Khoa?> GetByTenKhoaAsync(string tenKhoa)
    {
        return await _context.Khoas.FirstOrDefaultAsync(k => k.TenKhoa == tenKhoa);
    }
}