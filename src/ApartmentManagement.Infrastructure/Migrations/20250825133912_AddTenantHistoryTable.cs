using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoveInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoveOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeaseTerms = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsRenewed = table.Column<bool>(type: "bit", nullable: false),
                    LeaseRenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasEarlyTermination = table.Column<bool>(type: "bit", nullable: false),
                    WasEvicted = table.Column<bool>(type: "bit", nullable: false),
                    EvictionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectionReport = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApartmentConditionAtMoveIn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApartmentConditionAtMoveOut = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantHistories_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantHistories_ApartmentId",
                table: "TenantHistories",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantHistories_Email",
                table: "TenantHistories",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TenantHistories_TenantId_CreatedAt",
                table: "TenantHistories",
                columns: new[] { "TenantId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantHistories");
        }
    }
}
