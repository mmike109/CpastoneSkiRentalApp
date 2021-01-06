using Microsoft.EntityFrameworkCore.Migrations;

namespace CpastroneSkiRental.Data.Migrations
{
    public partial class itemsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Maker = table.Column<string>(maxLength: 45, nullable: false),
                    ItemType = table.Column<string>(maxLength: 45, nullable: false),
                    ItemGender = table.Column<string>(maxLength: 8, nullable: false),
                    ItemSize = table.Column<int>(nullable: false),
                    ItemAltSize = table.Column<string>(maxLength: 2, nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Level = table.Column<string>(maxLength: 20, nullable: false),
                    ItemStatus = table.Column<string>(maxLength: 20, nullable: false),
                    ItemImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item");
        }
    }
}
