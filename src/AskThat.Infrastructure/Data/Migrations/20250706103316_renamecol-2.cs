using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskThat.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class renamecol2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentCount",
                table: "Questions",
                newName: "AnswerCount");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnswerCount",
                table: "Questions",
                newName: "CommentCount");

        }
    }
}
