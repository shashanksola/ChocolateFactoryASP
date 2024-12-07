using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChocolateFactory.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRawMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RawMaterials",
                table: "RawMaterials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RawMaterials",
                table: "RawMaterials",
                column: "RawMaterialBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RawMaterials",
                table: "RawMaterials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RawMaterials",
                table: "RawMaterials",
                column: "Name");
        }
    }
}
