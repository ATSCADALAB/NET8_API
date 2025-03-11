using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickStart.Migrations
{
    /// <inheritdoc />
    public partial class Add_Orders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "29d0e784-7b0c-409d-9bb9-7016d2aa97c5");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e447ce42-c4ec-47d5-934b-6d71dee428f0");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExportDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    QuantityVehicle = table.Column<int>(type: "int", nullable: false),
                    VehicleNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContainerNumber = table.Column<int>(type: "int", nullable: false),
                    SealNumber = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverPhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnitOrder = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeightOrder = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductInformationID = table.Column<long>(type: "bigint", nullable: false),
                    DistributorID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_Distributors_DistributorID",
                        column: x => x.DistributorID,
                        principalTable: "Distributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_ProductInformations_ProductInformationID",
                        column: x => x.ProductInformationID,
                        principalTable: "ProductInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "DateCreated", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "394c48c0-9849-4af2-8394-4dbee6e54aaf", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "User", "USER" },
                    { "462236e3-8202-4910-a510-bb6227375b9e", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DistributorID",
                table: "Orders",
                column: "DistributorID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductInformationID",
                table: "Orders",
                column: "ProductInformationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "394c48c0-9849-4af2-8394-4dbee6e54aaf");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "462236e3-8202-4910-a510-bb6227375b9e");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "DateCreated", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "29d0e784-7b0c-409d-9bb9-7016d2aa97c5", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "User", "USER" },
                    { "e447ce42-c4ec-47d5-934b-6d71dee428f0", null, new DateTime(2015, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "UserRole", "Admin", "ADMIN" }
                });
        }
    }
}
