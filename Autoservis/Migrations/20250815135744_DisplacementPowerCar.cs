using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autoservis.Migrations
{
    /// <inheritdoc />
    public partial class DisplacementPowerCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplacementPower",
                table: "Cars",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplacementPower",
                table: "Cars");
        }
    }
}
