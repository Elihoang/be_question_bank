using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Models;
using File = BEQuestionBank.Domain.Models.File;
using BEQuestionBank.Core.Seed;

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
    public DbSet<YeuCauRutTrich> YeuCauRutTrichs { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<MonHoc>()
        .HasIndex(m => m.MaSoMonHoc)
        .IsUnique();
        
        // KhoaSeed.Seed(modelBuilder);
        modelBuilder.Entity<CauHoi>()
            .HasOne(c => c.CauHoiCha)
            .WithMany(c => c.CauHoiCons)
            .HasForeignKey(c => c.MaCauHoiCha)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ChiTietDeThi>()
            .HasKey(ct => new { ct.MaDeThi, ct.MaCauHoi });

    }
}
