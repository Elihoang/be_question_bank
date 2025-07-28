using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Khoa",
                columns: table => new
                {
                    MaKhoa = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKhoa = table.Column<string>(type: "text", nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: false),
                    XoaTamKhoa = table.Column<bool>(type: "boolean", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoa", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "MonHoc",
                columns: table => new
                {
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenMonHoc = table.Column<string>(type: "text", nullable: false),
                    MaSoMonHoc = table.Column<string>(type: "text", nullable: false),
                    SoTinChi = table.Column<int>(type: "integer", nullable: true),
                    XoaTamMonHoc = table.Column<bool>(type: "boolean", nullable: true),
                    MaKhoa = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHoc", x => x.MaMonHoc);
                    table.ForeignKey(
                        name: "FK_MonHoc_Khoa_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    VaiTro = table.Column<int>(type: "integer", nullable: false),
                    BiKhoa = table.Column<bool>(type: "boolean", nullable: false),
                    MaKhoa = table.Column<string>(type: "text", nullable: true),
                    KhoaMaKhoa = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_Khoa_KhoaMaKhoa",
                        column: x => x.KhoaMaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa");
                });

            migrationBuilder.CreateTable(
                name: "DeThi",
                columns: table => new
                {
                    MaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDeThi = table.Column<string>(type: "text", nullable: false),
                    DaDuyet = table.Column<bool>(type: "boolean", nullable: false),
                    SoCauHoi = table.Column<int>(type: "integer", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_DeThi_MonHoc_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHoc",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phan",
                columns: table => new
                {
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    TenPhan = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    ThuTu = table.Column<int>(type: "integer", nullable: false),
                    SoLuongCauHoi = table.Column<int>(type: "integer", nullable: false),
                    MaPhanCha = table.Column<Guid>(type: "uuid", nullable: true),
                    MaSoPhan = table.Column<int>(type: "integer", nullable: true),
                    XoaTamPhan = table.Column<bool>(type: "boolean", nullable: true),
                    LaCauHoiNhom = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phan", x => x.MaPhan);
                    table.ForeignKey(
                        name: "FK_Phan_MonHoc_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHoc",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
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
                    TacNhanNguoiDung = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_AuditLogs_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "YeuCauRutTrich",
                columns: table => new
                {
                    MaYeuCau = table.Column<Guid>(type: "uuid", nullable: false),
                    MaNguoiDung = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDungRutTrich = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayYeuCau = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DaXuLy = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauRutTrich", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauRutTrich_MonHoc_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHoc",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YeuCauRutTrich_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSoCauHoi = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    HoanVi = table.Column<bool>(type: "boolean", nullable: false),
                    CapDo = table.Column<short>(type: "smallint", nullable: false),
                    SoCauHoiCon = table.Column<int>(type: "integer", nullable: false),
                    MaCauHoiCha = table.Column<Guid>(type: "uuid", nullable: true),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false),
                    SoLanDuocThi = table.Column<int>(type: "integer", nullable: true),
                    SoLanDung = table.Column<int>(type: "integer", nullable: true),
                    DoPhanCachCauHoi = table.Column<float>(type: "real", nullable: true),
                    XoaTam = table.Column<bool>(type: "boolean", nullable: true),
                    CLO = table.Column<int>(type: "integer", nullable: true),
                    NgaySua = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uuid", nullable: true),
                    CauHoiChaMaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.MaCauHoi);
                    table.ForeignKey(
                        name: "FK_CauHoi_CauHoi_CauHoiChaMaCauHoi",
                        column: x => x.CauHoiChaMaCauHoi,
                        principalTable: "CauHoi",
                        principalColumn: "MaCauHoi");
                    table.ForeignKey(
                        name: "FK_CauHoi_NguoiDung_NguoiTao",
                        column: x => x.NguoiTao,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_CauHoi_Phan_MaPhan",
                        column: x => x.MaPhan,
                        principalTable: "Phan",
                        principalColumn: "MaPhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauTraLoi",
                columns: table => new
                {
                    MaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    ThuTu = table.Column<int>(type: "integer", nullable: false),
                    HoanVi = table.Column<bool>(type: "boolean", nullable: true),
                    LaDapAn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTraLoi", x => x.MaCauTraLoi);
                    table.ForeignKey(
                        name: "FK_CauTraLoi_CauHoi_MaCauHoi",
                        column: x => x.MaCauHoi,
                        principalTable: "CauHoi",
                        principalColumn: "MaCauHoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDeThi",
                columns: table => new
                {
                    MaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: false),
                    ThuTu = table.Column<int>(type: "integer", nullable: true),
                    DeThiMaDeThi = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDeThi", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThi_CauHoi_MaCauHoi",
                        column: x => x.MaCauHoi,
                        principalTable: "CauHoi",
                        principalColumn: "MaCauHoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThi_DeThi_DeThiMaDeThi",
                        column: x => x.DeThiMaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThi_Phan_MaPhan",
                        column: x => x.MaPhan,
                        principalTable: "Phan",
                        principalColumn: "MaPhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    MaFile = table.Column<Guid>(type: "uuid", nullable: false),
                    MaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    TenFile = table.Column<string>(type: "text", nullable: true),
                    LoaiFile = table.Column<int>(type: "integer", nullable: true),
                    MaCauTraLoi = table.Column<string>(type: "text", nullable: true),
                    CauTraLoiMaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.MaFile);
                    table.ForeignKey(
                        name: "FK_Files_CauHoi_MaCauHoi",
                        column: x => x.MaCauHoi,
                        principalTable: "CauHoi",
                        principalColumn: "MaCauHoi");
                    table.ForeignKey(
                        name: "FK_Files_CauTraLoi_CauTraLoiMaCauTraLoi",
                        column: x => x.CauTraLoiMaCauTraLoi,
                        principalTable: "CauTraLoi",
                        principalColumn: "MaCauTraLoi");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_MaNguoiDung",
                table: "AuditLogs",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_CauHoiChaMaCauHoi",
                table: "CauHoi",
                column: "CauHoiChaMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_MaPhan",
                table: "CauHoi",
                column: "MaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_NguoiTao",
                table: "CauHoi",
                column: "NguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_MaCauHoi",
                table: "CauTraLoi",
                column: "MaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_DeThiMaDeThi",
                table: "ChiTietDeThi",
                column: "DeThiMaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_MaCauHoi",
                table: "ChiTietDeThi",
                column: "MaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_MaPhan",
                table: "ChiTietDeThi",
                column: "MaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_MaMonHoc",
                table: "DeThi",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CauTraLoiMaCauTraLoi",
                table: "Files",
                column: "CauTraLoiMaCauTraLoi");

            migrationBuilder.CreateIndex(
                name: "IX_Files_MaCauHoi",
                table: "Files",
                column: "MaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_MonHoc_MaKhoa",
                table: "MonHoc",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_MonHoc_MaSoMonHoc",
                table: "MonHoc",
                column: "MaSoMonHoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_KhoaMaKhoa",
                table: "NguoiDung",
                column: "KhoaMaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_Phan_MaMonHoc",
                table: "Phan",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauRutTrich_MaMonHoc",
                table: "YeuCauRutTrich",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauRutTrich_MaNguoiDung",
                table: "YeuCauRutTrich",
                column: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ChiTietDeThi");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "YeuCauRutTrich");

            migrationBuilder.DropTable(
                name: "DeThi");

            migrationBuilder.DropTable(
                name: "CauTraLoi");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Phan");

            migrationBuilder.DropTable(
                name: "MonHoc");

            migrationBuilder.DropTable(
                name: "Khoa");
        }
    }
}
