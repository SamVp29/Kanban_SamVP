using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanban.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedToBcrypt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "$2a$11$0lA2YO/An/7Ybh/43wcH8.XgHkRCPlybm5uE1/LUJA8GssgLiiMZe", "" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "$2a$11$0lA2YO/An/7Ybh/43wcH8.XgHkRCPlybm5uE1/LUJA8GssgLiiMZe", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "b3SfDoTNb9lwJ1w00E5G+IfazPui+eb0vRdWvKuYAUg=", "b5150290-b42c-4e79-80e0-9a8230eb1954" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "eeDElP8PIWkTOD99uVGBIIdLjj17tZo8ta8uXYhZvhM=", "c5b059a5-3ad6-45a4-b4b5-fcc05ec09721" });
        }
    }
}
