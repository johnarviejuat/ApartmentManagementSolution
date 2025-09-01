using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leasing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeasingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Leases_PreviousLeaseId",
                schema: "leasing",
                table: "Leases",
                column: "PreviousLeaseId",
                unique: true,
                filter: "[PreviousLeaseId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leases_Dates_Order",
                schema: "leasing",
                table: "Leases",
                sql: "[EndDate] IS NULL OR [EndDate] >= [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leases_Deposits_NonNegative",
                schema: "leasing",
                table: "Leases",
                sql: "[DepositRequired] >= 0 AND [DepositHeld] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Leases_PreviousLeaseId",
                schema: "leasing",
                table: "Leases",
                column: "PreviousLeaseId",
                principalSchema: "leasing",
                principalTable: "Leases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Leases_PreviousLeaseId",
                schema: "leasing",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_PreviousLeaseId",
                schema: "leasing",
                table: "Leases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Leases_Dates_Order",
                schema: "leasing",
                table: "Leases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Leases_Deposits_NonNegative",
                schema: "leasing",
                table: "Leases");
        }
    }
}
