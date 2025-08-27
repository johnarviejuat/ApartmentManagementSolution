using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leases_TenantId_ApartmentId",
                table: "Leases");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Leases",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Leases",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousLeaseId",
                table: "Leases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Leases",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Leases_ApartmentId_IsActive",
                table: "Leases",
                columns: new[] { "ApartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId_ApartmentId",
                table: "Leases",
                columns: new[] { "TenantId", "ApartmentId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId_IsActive",
                table: "Leases",
                columns: new[] { "TenantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leases_ApartmentId_IsActive",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_TenantId_ApartmentId",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_TenantId_IsActive",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "PreviousLeaseId",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Leases");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Leases",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId_ApartmentId",
                table: "Leases",
                columns: new[] { "TenantId", "ApartmentId" },
                unique: true);
        }
    }
}
