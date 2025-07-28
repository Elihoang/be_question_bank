using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Core.Seed;

public static class KhoaSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Khoa>().HasData(
            new Khoa
            {
                MaKhoa = Guid.NewGuid(),
                TenKhoa = "Khoa Công nghệ Thông tin",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c300"),
                TenKhoa = "Khoa Tài chính - Thương mại",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c301"),
                TenKhoa = "Khoa Kiến trúc - Mỹ thuật",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c302"),
                TenKhoa = "Khoa QT Du lịch - Nhà hàng - Khách sạn",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c303"),
                TenKhoa = "Khoa Tiếng Anh",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c304"),
                TenKhoa = "Khoa Nhật Bản học",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c305"),
                TenKhoa = "Khoa Xây dựng",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c306"),
                TenKhoa = "Khoa Luật",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c307"),
                TenKhoa = "Khoa Dược",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c308"),
                TenKhoa = "Khoa Hệ thống thông tin quản lý",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c309"),
                TenKhoa = "Khoa Truyền thông - Thiết kế",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c310"),
                TenKhoa = "Viện Kỹ thuật HUTECH",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c311"),
                TenKhoa = "Viện Khoa học ứng dụng HUTECH",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c312"),
                TenKhoa = "Viện Khoa học Xã hội và Nhân văn",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c313"),
                TenKhoa = "Viện Đào tạo Quốc tế HUTECH",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c314"),
                TenKhoa = "Viện Công nghệ Việt - Hàn",
                XoaTam = false
            }, 
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c315"),
                TenKhoa = "Viện Công nghệ Việt - Nhật",
                XoaTam = false
            },
            new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c316"),
                TenKhoa = "Trung tâm Tin học - Ngoại ngữ - Kỹ năng",
                XoaTam = false
            }, new Khoa
            {
                MaKhoa = Guid.Parse("0dd0e3f2-33b4-448a-a43a-2b5dd691c317"),
                TenKhoa = "TT Giáo dục chính trị - Quốc phòng",
                XoaTam = false
            }

        );
    }
}
