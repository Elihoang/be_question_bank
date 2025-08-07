using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class ngaydangnhapuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<Guid>(
                name: "MaKhoa",
                table: "NguoiDung",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDangNhapCuoi",
                table: "NguoiDung",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaKhoa",
                table: "NguoiDung",
                column: "MaKhoa");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_Khoa_MaKhoa",
                table: "NguoiDung",
                column: "MaKhoa",
                principalTable: "Khoa",
                principalColumn: "MaKhoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_Khoa_MaKhoa",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_MaKhoa",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayDangNhapCuoi",
                table: "NguoiDung");

            migrationBuilder.AlterColumn<string>(
                name: "MaKhoa",
                table: "NguoiDung",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhoaMaKhoa",
                table: "NguoiDung",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_KhoaMaKhoa",
                table: "NguoiDung",
                column: "KhoaMaKhoa");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_Khoa_KhoaMaKhoa",
                table: "NguoiDung",
                column: "KhoaMaKhoa",
                principalTable: "Khoa",
                principalColumn: "MaKhoa");
        }
    }
}
