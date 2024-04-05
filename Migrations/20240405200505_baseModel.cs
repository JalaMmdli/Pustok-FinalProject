using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pustok.Migrations
{
    public partial class baseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Sliders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Service",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "ProductImgs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "BasketItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "ProductImgs");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "BasketItems");
        }
    }
}
