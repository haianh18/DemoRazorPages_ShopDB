using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoRazorPages_ShopDB.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderNote",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNote",
                table: "Order");
        }
    }
}
