using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BEQuestionBank.Core.Migrations
{
    /// <inheritdoc />
    public partial class TenMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaTran",
                table: "YeuCauRutTrich",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaTran",
                table: "YeuCauRutTrich");
        }
    }
}
