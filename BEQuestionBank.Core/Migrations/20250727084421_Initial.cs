using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    KhoaMaKhoa = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHoc", x => x.MaMonHoc);
                    table.ForeignKey(
                        name: "FK_MonHoc_Khoa_KhoaMaKhoa",
                        column: x => x.KhoaMaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa");
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
                    MonHocMaMonHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_DeThi_MonHoc_MonHocMaMonHoc",
                        column: x => x.MonHocMaMonHoc,
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
                    MonHocMaMonHoc = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phan", x => x.MaPhan);
                    table.ForeignKey(
                        name: "FK_Phan_MonHoc_MonHocMaMonHoc",
                        column: x => x.MonHocMaMonHoc,
                        principalTable: "MonHoc",
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
                    TacNhanNguoiDung = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_AuditLogs_NguoiDung_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
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
                    MaCauHoiCha = table.Column<string>(type: "text", nullable: true),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false),
                    SoLanDuocThi = table.Column<int>(type: "integer", nullable: false),
                    SoLanDung = table.Column<int>(type: "integer", nullable: false),
                    DoPhanCachCauHoi = table.Column<float>(type: "real", nullable: true),
                    ChuanDauRa = table.Column<int>(type: "integer", nullable: true),
                    NgaySua = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uuid", nullable: true),
                    PhanMaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiChaMaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uuid", nullable: true),
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
                        name: "FK_CauHoi_NguoiDung_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_CauHoi_Phan_PhanMaPhan",
                        column: x => x.PhanMaPhan,
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
                    LaDapAn = table.Column<bool>(type: "boolean", nullable: false),
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTraLoi", x => x.MaCauTraLoi);
                    table.ForeignKey(
                        name: "FK_CauTraLoi_CauHoi_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
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
                    DeThiMaDeThi = table.Column<Guid>(type: "uuid", nullable: false),
                    PhanMaPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDeThi", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_ChiTietDeThi_CauHoi_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
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
                        name: "FK_ChiTietDeThi_Phan_PhanMaPhan",
                        column: x => x.PhanMaPhan,
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
                    CauHoiMaCauHoi = table.Column<Guid>(type: "uuid", nullable: true),
                    CauTraLoiMaCauTraLoi = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.MaFile);
                    table.ForeignKey(
                        name: "FK_Files_CauHoi_CauHoiMaCauHoi",
                        column: x => x.CauHoiMaCauHoi,
                        principalTable: "CauHoi",
                        principalColumn: "MaCauHoi");
                    table.ForeignKey(
                        name: "FK_Files_CauTraLoi_CauTraLoiMaCauTraLoi",
                        column: x => x.CauTraLoiMaCauTraLoi,
                        principalTable: "CauTraLoi",
                        principalColumn: "MaCauTraLoi");
                });

            migrationBuilder.InsertData(
                table: "Khoa",
                columns: new[] { "MaKhoa", "MoTa", "NgayCapNhap", "NgayTao", "TenKhoa", "XoaTamKhoa" },
                values: new object[,]
                {
                    { new Guid("13331579-a09e-4e0c-b5e7-fa9d8900f7a7"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2447), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2447), "Khoa QT Du lịch - Nhà hàng - Khách sạn", false },
                    { new Guid("199a0115-e4d5-46ef-82dc-b017d53c807b"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2644), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2643), "Viện Kỹ thuật HUTECH", false },
                    { new Guid("2938cbeb-53de-4f3b-9197-5c8f422c58e3"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2420), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2419), "Khoa Kiến trúc - Mỹ thuật", false },
                    { new Guid("46063d4c-4b70-45c5-b303-23265152a2d4"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2389), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2388), "Khoa Tài chính - Thương mại", false },
                    { new Guid("48241a01-93d8-4571-9d17-94c79adb8f03"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2689), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2689), "Viện Khoa học Xã hội và Nhân văn", false },
                    { new Guid("53a219ab-ed94-4d87-923a-b0ee267d3cf4"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2846), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2846), "Viện Đào tạo Quốc tế HUTECH", false },
                    { new Guid("753ab298-018a-4909-85ba-52fc46e0371e"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2621), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2620), "Khoa Truyền thông - Thiết kế", false },
                    { new Guid("86348359-803b-425c-9015-0f33b3d66a63"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(147), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(146), "Khoa Công nghệ Thông tin", false },
                    { new Guid("8bfb986d-9303-42b2-a95a-1e38f5362c45"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2915), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2915), "Trung tâm Tin học - Ngoại ngữ - Kỹ năng", false },
                    { new Guid("91acaf9b-6777-4ca3-86bb-4adf3d7791a2"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2893), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2893), "Viện Công nghệ Việt - Nhật", false },
                    { new Guid("abec0c99-965c-4ce2-b27d-ce89323bf33e"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2475), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2475), "Khoa Tiếng Anh", false },
                    { new Guid("c8318452-3049-4bf1-85aa-069e9582d0f5"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2502), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2502), "Khoa Nhật Bản học", false },
                    { new Guid("cac54872-b2dd-41ee-8121-a5ec2fe82b21"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2666), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2666), "Viện Khoa học ứng dụng HUTECH", false },
                    { new Guid("e40f9110-2bb6-45b1-ac6a-b3f6e1ad7f94"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2551), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2550), "Khoa Luật", false },
                    { new Guid("ea2906c8-5258-4d57-a809-cd45c13b407d"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2598), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2597), "Khoa Hệ thống thông tin quản lý", false },
                    { new Guid("f07f4e2d-afb1-4fa4-94d7-b9cbeef34c30"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2574), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2574), "Khoa Dược", false },
                    { new Guid("f38d581b-7163-45a0-ab20-10e83a16cd26"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2870), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2870), "Viện Công nghệ Việt - Hàn", false },
                    { new Guid("f9c44896-4255-4e9e-aea6-5fb957706477"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2528), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2528), "Khoa Xây dựng", false },
                    { new Guid("fa81d0f0-8b89-46e1-baac-ff79f9f60811"), "", new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2938), new DateTime(2025, 7, 27, 8, 44, 19, 455, DateTimeKind.Utc).AddTicks(2938), "TT Giáo dục chính trị - Quốc phòng", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_NguoiDungMaNguoiDung",
                table: "AuditLogs",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_CauHoiChaMaCauHoi",
                table: "CauHoi",
                column: "CauHoiChaMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_NguoiDungMaNguoiDung",
                table: "CauHoi",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_PhanMaPhan",
                table: "CauHoi",
                column: "PhanMaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_CauHoiMaCauHoi",
                table: "CauTraLoi",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_CauHoiMaCauHoi",
                table: "ChiTietDeThi",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_DeThiMaDeThi",
                table: "ChiTietDeThi",
                column: "DeThiMaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDeThi_PhanMaPhan",
                table: "ChiTietDeThi",
                column: "PhanMaPhan");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_MonHocMaMonHoc",
                table: "DeThi",
                column: "MonHocMaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CauHoiMaCauHoi",
                table: "Files",
                column: "CauHoiMaCauHoi");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CauTraLoiMaCauTraLoi",
                table: "Files",
                column: "CauTraLoiMaCauTraLoi");

            migrationBuilder.CreateIndex(
                name: "IX_MonHoc_KhoaMaKhoa",
                table: "MonHoc",
                column: "KhoaMaKhoa");

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
                name: "IX_Phan_MonHocMaMonHoc",
                table: "Phan",
                column: "MonHocMaMonHoc");
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
