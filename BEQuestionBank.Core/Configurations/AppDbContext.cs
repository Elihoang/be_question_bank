using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Core.Configurations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Khoa> Khoas { get; set; }
    public DbSet<MonHoc> MonHocs { get; set; }
    public DbSet<Phan> Phans { get; set; }
    public DbSet<NguoiDung> NguoiDungs { get; set; }
    public DbSet<CauHoi> CauHois { get; set; }
    public DbSet<CauTraLoi> CauTraLois { get; set; }
    public DbSet<DeThi> DeThis { get; set; }
    public DbSet<ChiTietDeThi> ChiTietDeThis { get; set; }
    public DbSet<GiangVienMonHoc> GiangVienMonHocs { get; set; }
    public DbSet<FileDinhKem> FileDinhKems { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
