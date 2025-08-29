using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leasing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitLeasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "leasing");

            migrationBuilder.CreateTable(
                name: "Leases",
                schema: "leasing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NextDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositRequired = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositHeld = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PreviousLeaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leases_ApartmentId_IsActive",
                schema: "leasing",
                table: "Leases",
                columns: new[] { "ApartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId_ApartmentId",
                schema: "leasing",
                table: "Leases",
                columns: new[] { "TenantId", "ApartmentId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId_IsActive",
                schema: "leasing",
                table: "Leases",
                columns: new[] { "TenantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leases",
                schema: "leasing");
        }
    }
}
