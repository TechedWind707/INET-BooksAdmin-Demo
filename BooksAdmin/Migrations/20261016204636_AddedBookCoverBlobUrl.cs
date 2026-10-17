using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddedBookCoverBlobUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedCoverUrl",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedCoverUrl",
                table: "Book");
        }
    }
}
