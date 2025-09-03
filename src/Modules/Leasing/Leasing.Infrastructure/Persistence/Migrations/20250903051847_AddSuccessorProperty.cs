using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leasing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessorProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SuccessorId",
                schema: "leasing",
                table: "Leases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leases_SuccessorId",
                schema: "leasing",
                table: "Leases",
                column: "SuccessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Leases_SuccessorId",
                schema: "leasing",
                table: "Leases",
                column: "SuccessorId",
                principalSchema: "leasing",
                principalTable: "Leases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Leases_SuccessorId",
                schema: "leasing",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_SuccessorId",
                schema: "leasing",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "SuccessorId",
                schema: "leasing",
                table: "Leases");
        }
    }
}
