using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodieHub.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestaurantImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    PriceRangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurants_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Restaurants_PriceRanges_PriceRangeId",
                        column: x => x.PriceRangeId,
                        principalTable: "PriceRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CityImageUrl", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("14ceba71-4b51-4777-9b17-46602cf66153"), null, "CHI", "Chicago" },
                    { new Guid("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"), null, "LA", "Los Angeles" },
                    { new Guid("906cb139-415a-4bbb-a174-1a1faf9fb1f6"), "https://images.pexels.com/photos/2422461/pexels-photo-2422461.jpeg", "MIA", "Miami" },
                    { new Guid("cfa06ed2-bf65-4b65-93ed-c9d286ddb0de"), "https://images.pexels.com/photos/1259279/pexels-photo-1259279.jpeg", "SF", "San Francisco" },
                    { new Guid("f077a22e-4248-4bf6-b564-c7cf4e250263"), null, "BOS", "Boston" },
                    { new Guid("f7248fc3-2585-4efb-8d1d-1c555f4087f6"), "https://images.pexels.com/photos/2190283/pexels-photo-2190283.jpeg", "NYC", "New York" }
                });

            migrationBuilder.InsertData(
                table: "PriceRanges",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"), "Budget-Friendly" },
                    { new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"), "Moderate" },
                    { new Guid("f808ddcd-b5e5-4d80-b732-1ca523e48434"), "Fine Dining" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_CityId",
                table: "Restaurants",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_PriceRangeId",
                table: "Restaurants",
                column: "PriceRangeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "PriceRanges");
        }
    }
}
