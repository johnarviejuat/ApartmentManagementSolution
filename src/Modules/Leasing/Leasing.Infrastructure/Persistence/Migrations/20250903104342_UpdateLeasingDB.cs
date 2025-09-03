using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leasing.Infrastructure.Persistence.Migrations
{
    public partial class UpdateLeasingDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the check constraint that references StartDate/EndDate
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leases_Dates_Order",
                table: "Leases",
                schema: "leasing");

            // Switch DateOnly-backed columns to datetime2
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "leasing",
                table: "Leases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextDueDate",
                schema: "leasing",
                table: "Leases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "leasing",
                table: "Leases",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            // Recreate the check constraint
            migrationBuilder.AddCheckConstraint(
                name: "CK_Leases_Dates_Order",
                table: "Leases",
                schema: "leasing",
                sql: "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the constraint first
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leases_Dates_Order",
                table: "Leases",
                schema: "leasing");

            // Revert columns back to SQL 'date'
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "leasing",
                table: "Leases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextDueDate",
                schema: "leasing",
                table: "Leases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "leasing",
                table: "Leases",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            // Re-add the original constraint
            migrationBuilder.AddCheckConstraint(
                name: "CK_Leases_Dates_Order",
                table: "Leases",
                schema: "leasing",
                sql: "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
        }
    }
}
