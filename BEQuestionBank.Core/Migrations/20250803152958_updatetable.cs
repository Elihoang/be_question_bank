using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDeThi",
                table: "ChiTietDeThi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDeThi",
                table: "ChiTietDeThi",
                columns: new[] { "MaDeThi", "MaCauHoi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDeThi",
                table: "ChiTietDeThi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDeThi",
                table: "ChiTietDeThi",
                column: "MaDeThi");
        }
    }
}
