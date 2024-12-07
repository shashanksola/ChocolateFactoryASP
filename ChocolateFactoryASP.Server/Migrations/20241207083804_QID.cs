using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChocolateFactory.Migrations
{
    /// <inheritdoc />
    public partial class QID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QualityChecks_Users_QualityControllerNameUserId",
                table: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_QualityControllerNameUserId",
                table: "QualityChecks");

            migrationBuilder.RenameColumn(
                name: "QualityControllerNameUserId",
                table: "QualityChecks",
                newName: "QualityControllerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QualityControllerId",
                table: "QualityChecks",
                newName: "QualityControllerNameUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_QualityControllerNameUserId",
                table: "QualityChecks",
                column: "QualityControllerNameUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QualityChecks_Users_QualityControllerNameUserId",
                table: "QualityChecks",
                column: "QualityControllerNameUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
