using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTableService.Migrations
{
    /// <inheritdoc />
    public partial class updategametablestarttimename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "startDateTime",
                table: "GameTables",
                newName: "StartDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "GameTables",
                newName: "startDateTime");
        }
    }
}
