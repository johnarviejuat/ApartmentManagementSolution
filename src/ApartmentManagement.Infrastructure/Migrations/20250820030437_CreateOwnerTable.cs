using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateOwnerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId1",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_OwnerId1",
                table: "Apartments",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Owners_OwnerId1",
                table: "Apartments",
                column: "OwnerId1",
                principalTable: "Owners",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Owners_OwnerId1",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_OwnerId1",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Apartments");
        }
    }
}
