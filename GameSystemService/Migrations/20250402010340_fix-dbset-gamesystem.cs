using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameSystemService.Migrations
{
    /// <inheritdoc />
    public partial class fixdbsetgamesystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gameSystems",
                table: "gameSystems");

            migrationBuilder.RenameTable(
                name: "gameSystems",
                newName: "GameSystems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameSystems",
                table: "GameSystems",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameSystems",
                table: "GameSystems");

            migrationBuilder.RenameTable(
                name: "GameSystems",
                newName: "gameSystems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gameSystems",
                table: "gameSystems",
                column: "Id");
        }
    }
}
