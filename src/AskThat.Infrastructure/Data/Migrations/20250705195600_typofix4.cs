using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskThat.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class typofix4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Questions",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Questions",
                newName: "UpdateAt");
        }
    }
}
