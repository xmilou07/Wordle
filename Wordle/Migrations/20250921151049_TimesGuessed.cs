using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wordle.Migrations
{
    /// <inheritdoc />
    public partial class TimesGuessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesGuessed",
                table: "Word",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesGuessed",
                table: "Word");
        }
    }
}
