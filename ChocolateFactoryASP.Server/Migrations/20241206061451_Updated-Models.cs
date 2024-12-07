using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChocolateFactory.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductionSchedules");

            migrationBuilder.RenameColumn(
                name: "InspectorId",
                table: "QualityChecks",
                newName: "QualityControllerNameUserId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Recipes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipeName",
                table: "ProductionSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Name");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QualityChecks_Users_QualityControllerNameUserId",
                table: "QualityChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_QualityControllerNameUserId",
                table: "QualityChecks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeName",
                table: "ProductionSchedules");

            migrationBuilder.RenameColumn(
                name: "QualityControllerNameUserId",
                table: "QualityChecks",
                newName: "InspectorId");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "ProductionSchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "RecipeId");
        }
    }
}
