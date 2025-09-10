using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autoservis.Migrations
{
    /// <inheritdoc />
    public partial class MaterialSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Supplier",
                table: "Materials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Materials");
        }
    }
}
