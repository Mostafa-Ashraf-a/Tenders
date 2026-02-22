using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderingSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryFieldsToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Tenders");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Tenders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCategories",
                columns: table => new
                {
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCategories", x => new { x.SupplierId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_SupplierCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierCategories_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_CategoryId",
                table: "Tenders",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_CategoryId",
                table: "SupplierCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Categories_CategoryId",
                table: "Tenders",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Categories_CategoryId",
                table: "Tenders");

            migrationBuilder.DropTable(
                name: "SupplierCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_CategoryId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tenders");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Tenders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
