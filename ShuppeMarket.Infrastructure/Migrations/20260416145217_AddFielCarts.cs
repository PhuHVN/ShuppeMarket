using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShuppeMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFielCarts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Carts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Carts");
        }
    }
}
