using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    public partial class CommentsNewId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutorName",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "AutorProfileImage",
                table: "Comments",
                newName: "AutorId");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "AutorId",
                table: "Comments",
                newName: "AutorProfileImage");

            migrationBuilder.AddColumn<string>(
                name: "AutorName",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
