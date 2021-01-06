using Microsoft.EntityFrameworkCore.Migrations;

namespace CpastroneSkiRental.Data.Migrations
{
    public partial class sizeChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ItemSize",
                table: "Item",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ItemSize",
                table: "Item",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
