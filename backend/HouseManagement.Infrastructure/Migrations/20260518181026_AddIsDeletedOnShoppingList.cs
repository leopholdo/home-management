using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedOnShoppingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShoppingList",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShoppingList");
        }
    }
}
