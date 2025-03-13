using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickStart.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLineDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "394c48c0-9849-4af2-8394-4dbee6e54aaf");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "462236e3-8202-4910-a510-bb6227375b9e");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.CreateTable(
                name: "OrderLineDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    Line = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLineDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLineDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "DateCreated", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f35bced-06f3-4939-aa90-38ac9d1bb8db", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "User", "USER" },
                    { "bef8f545-49e3-4740-b0d5-7efeb000ec10", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineDetails_OrderId",
                table: "OrderLineDetails",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLineDetails");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "5f35bced-06f3-4939-aa90-38ac9d1bb8db");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "bef8f545-49e3-4740-b0d5-7efeb000ec10");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "DateCreated", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "394c48c0-9849-4af2-8394-4dbee6e54aaf", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "User", "USER" },
                    { "462236e3-8202-4910-a510-bb6227375b9e", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "Admin", "ADMIN" }
                });
        }
    }
}
