using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCauHoiMaCauHoiFromDeThi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeThi_CauHoi_CauHoiMaCauHoi",
                table: "DeThi");

            migrationBuilder.DropIndex(
                name: "IX_DeThi_CauHoiMaCauHoi",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "CauHoiMaCauHoi",
                table: "DeThi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CauHoiMaCauHoi",
                table: "DeThi",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_CauHoiMaCauHoi",
                table: "DeThi",
                column: "CauHoiMaCauHoi");

            migrationBuilder.AddForeignKey(
                name: "FK_DeThi_CauHoi_CauHoiMaCauHoi",
                table: "DeThi",
                column: "CauHoiMaCauHoi",
                principalTable: "CauHoi",
                principalColumn: "MaCauHoi");
        }
    }
}
