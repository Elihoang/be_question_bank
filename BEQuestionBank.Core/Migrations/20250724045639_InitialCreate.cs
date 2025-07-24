using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Khoas",
                columns: table => new
                {
                    MaKhoa = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKhoa = table.Column<string>(type: "text", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoas", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "MonHocs",
                columns: table => new
                {
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenMonHoc = table.Column<string>(type: "text", nullable: false),
                    MaSoMonHoc = table.Column<string>(type: "text", nullable: false),
                    MaKhoa = table.Column<Guid>(type: "uuid", nullable: false),
                    KhoaMaKhoa = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHocs", x => x.MaMonHoc);
                    table.ForeignKey(
                        name: "FK_MonHocs_Khoas_KhoaMaKhoa",
                        column: x => x.KhoaMaKhoa,
                        principalTable: "Khoas",
                        principalColumn: "MaKhoa");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    VaiTro = table.Column<int>(type: "integer", nullable: false),
                    BiKhoa = table.Column<bool>(type: "boolean", nullable: false),
                    MaKhoa = table.Column<Guid>(type: "uuid", nullable: true),
                    KhoaMaKhoa = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_Khoas_KhoaMaKhoa",
                        column: x => x.KhoaMaKhoa,
                        principalTable: "Khoas",
                        principalColumn: "MaKhoa");
                });

            migrationBuilder.CreateTable(
                name: "DeThis",
                columns: table => new
                {
                    MaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDeThi = table.Column<string>(type: "text", nullable: false),
                    DaDuyet = table.Column<bool>(type: "boolean", nullable: false),
                    SoCauHoi = table.Column<int>(type: "integer", nullable: true),
                    MonHocMaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThis", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_DeThis_MonHocs_MonHocMaMonHoc",
                        column: x => x.MonHocMaMonHoc,
                        principalTable: "MonHocs",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phans",
                columns: table => new
                {
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenPhan = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    ThuTu = table.Column<int>(type: "integer", nullable: false),
                    LaCauHoiNhom = table.Column<bool>(type: "boolean", nullable: false),
                    MonHocMaMonHoc = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phans", x => x.MaPhan);
                    table.ForeignKey(
                        name: "FK_Phans_MonHocs_MonHocMaMonHoc",
                        column: x => x.MonHocMaMonHoc,
                        principalTable: "MonHocs",
                        principalColumn: "MaMonHoc");
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    MaNhatKy = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenBang = table.Column<string>(type: "text", nullable: false),
                    MaBanGhi = table.Column<string>(type: "text", nullable: false),
                    HanhDong = table.Column<string>(type: "text", nullable: false),
                    GiaTriCu = table.Column<string>(type: "text", nullable: true),
                    GiaTriMoi = table.Column<string>(type: "text", nullable: true),
                    MaNguoiDung = table.Column<Guid>(type: "uuid", nullable: true),
                    TenNguoiDung = table.Column<string>(type: "text", nullable: true),
                    ThoiGianThucHien = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiaChiIP = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_AuditLogs_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "GiangVienMonHocs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DenNgay = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uuid", nullable: false),
                    MonHocMaMonHoc = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiangVienMonHocs", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_GiangVienMonHocs_MonHocs_MonHocMaMonHoc",
                        column: x => x.MonHocMaMonHoc,
                        principalTable: "MonHocs",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiangVienMonHocs_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHois",
                columns: table => new
                {
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSoCauHoi = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    HoanVi = table.Column<bool>(type: "boolean", nullable: false),
                    CapDo = table.Column<short>(type: "smallint", nullable: false),
                    LaCauHoiNhom = table.Column<bool>(type: "boolean", nullable: false),
                    MaCauHoiCha = table.Column<Guid>(type: "uuid", nullable: true),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false),
                    SoLanDuocThi = table.Column<int>(type: "integer", nullable: false),
                    SoLanDung = table.Column<int>(type: "integer", nullable: false),
                    DoPhanCach = table.Column<float>(type: "real", nullable: true),
                    NgaySua = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uuid", nullable: true),
                    PhanMaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiChaMaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHois", x => x.MaCauHoi);
                    table.ForeignKey(
                        name: "FK_CauHois_CauHois_CauHoiChaMaCauHoi",
                        column: x => x.CauHoiChaMaCauHoi,
                        principalTable: "CauHois",
                        principalColumn: "MaCauHoi");
                    table.ForeignKey(
                        name: "FK_CauHois_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_CauHois_Phans_PhanMaPhan",
                        column: x => x.PhanMaPhan,
                        principalTable: "Phans",
                        principalColumn: "MaPhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauTraLois",
                columns: table => new
                {
                    MaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    ThuTu = table.Column<int>(type: "integer", nullable: false),
                    LaDapAn = table.Column<bool>(type: "boolean", nullable: false),
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTraLois", x => x.MaCauTraLoi);
                    table.ForeignKey(
                        name: "FK_CauTraLois_CauHois_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
                        principalTable: "CauHois",
                        principalColumn: "MaCauHoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDeThis",
                columns: table => new
                {
                    MaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    ThuTu = table.Column<int>(type: "integer", nullable: true),
                    DeThiMaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    PhanMaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDeThis", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThis_CauHois_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
                        principalTable: "CauHois",
                        principalColumn: "MaCauHoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThis_DeThis_DeThiMaDeThi",
                        column: x => x.DeThiMaDeThi,
                        principalTable: "DeThis",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThis_Phans_PhanMaPhan",
                        column: x => x.PhanMaPhan,
                        principalTable: "Phans",
                        principalColumn: "MaPhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileDinhKems",
                columns: table => new
                {
                    MaFile = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    MaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: true),
                    TenFile = table.Column<string>(type: "text", nullable: true),
                    DuongDan = table.Column<string>(type: "text", nullable: true),
                    LoaiFile = table.Column<int>(type: "integer", nullable: true),
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    CauTraLoiMaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDinhKems", x => x.MaFile);
                    table.ForeignKey(
                        name: "FK_FileDinhKems_CauHois_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
                        principalTable: "CauHois",
                        principalColumn: "MaCauHoi");
                    table.ForeignKey(
                        name: "FK_FileDinhKems_CauTraLois_CauTraLoiMaCauTraLoi",
                        column: x => x.CauTraLoiMaCauTraLoi,
                        principalTable: "CauTraLois",
                        principalColumn: "MaCauTraLoi");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_NguoiDungMaNguoiDung",
                table: "AuditLogs",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CauHois_CauHoiChaMaCauHoi",
                table: "CauHois",
                column: "CauHoiChaMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_CauHois_NguoiDungMaNguoiDung",
                table: "CauHois",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CauHois_PhanMaPhan",
                table: "CauHois",
                column: "PhanMaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLois_CauHoiMaCauHoi",
                table: "CauTraLois",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThis_CauHoiMaCauHoi",
                table: "ChiTietDeThis",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThis_DeThiMaDeThi",
                table: "ChiTietDeThis",
                column: "DeThiMaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThis_PhanMaPhan",
                table: "ChiTietDeThis",
                column: "PhanMaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_DeThis_MonHocMaMonHoc",
                table: "DeThis",
                column: "MonHocMaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_FileDinhKems_CauHoiMaCauHoi",
                table: "FileDinhKems",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_FileDinhKems_CauTraLoiMaCauTraLoi",
                table: "FileDinhKems",
                column: "CauTraLoiMaCauTraLoi");

            migrationBuilder.CreateIndex(
                name: "IX_GiangVienMonHocs_MonHocMaMonHoc",
                table: "GiangVienMonHocs",
                column: "MonHocMaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_GiangVienMonHocs_NguoiDungMaNguoiDung",
                table: "GiangVienMonHocs",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_MonHocs_KhoaMaKhoa",
                table: "MonHocs",
                column: "KhoaMaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_KhoaMaKhoa",
                table: "NguoiDungs",
                column: "KhoaMaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_Phans_MonHocMaMonHoc",
                table: "Phans",
                column: "MonHocMaMonHoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ChiTietDeThis");

            migrationBuilder.DropTable(
                name: "FileDinhKems");

            migrationBuilder.DropTable(
                name: "GiangVienMonHocs");

            migrationBuilder.DropTable(
                name: "DeThis");

            migrationBuilder.DropTable(
                name: "CauTraLois");

            migrationBuilder.DropTable(
                name: "CauHois");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "Phans");

            migrationBuilder.DropTable(
                name: "MonHocs");

            migrationBuilder.DropTable(
                name: "Khoas");
        }
    }
}
