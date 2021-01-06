using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CpastroneSkiRental.Data.Migrations
{
    public partial class rental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deal",
                columns: table => new
                {
                    DealId = table.Column<Guid>(nullable: false),
                    ItemDescription = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    RentType = table.Column<string>(nullable: false),
                    ItemSize = table.Column<double>(nullable: false),
                    ItemAltSize = table.Column<string>(maxLength: 2, nullable: true),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deal", x => x.DealId);
                });

            migrationBuilder.CreateTable(
                name: "Rental",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(nullable: false),
                    DealId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 40, nullable: false),
                    ItemDescription = table.Column<string>(maxLength: 100, nullable: false),
                    RentalStartDate = table.Column<DateTime>(nullable: false),
                    RentalReturnDate = table.Column<DateTime>(nullable: false),
                    DayesRented = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rental", x => x.ProductId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropTable(
                name: "Rental");
        }
    }
}
