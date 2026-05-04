using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedApiKeysConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRecovered",
                table: "RepaymentSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Recover",
                table: "RepaymentSchedule",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandateDirectDebitApproved",
                table: "LoanRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MandateDirectDebitApproval",
                table: "LoanRequest",
                type: "nvarchar(3000)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientApiKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(400)", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApiKey", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2024, 9, 7, 19, 49, 18, 945, DateTimeKind.Utc).AddTicks(7654), new DateTime(2024, 9, 7, 19, 49, 18, 945, DateTimeKind.Utc).AddTicks(7657) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2024, 9, 7, 19, 49, 18, 903, DateTimeKind.Utc).AddTicks(6556));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientApiKey");

            migrationBuilder.DropColumn(
                name: "IsRecovered",
                table: "RepaymentSchedule");

            migrationBuilder.DropColumn(
                name: "Recover",
                table: "RepaymentSchedule");

            migrationBuilder.DropColumn(
                name: "IsMandateDirectDebitApproved",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "MandateDirectDebitApproval",
                table: "LoanRequest");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2024, 5, 20, 15, 17, 39, 37, DateTimeKind.Utc).AddTicks(4208), new DateTime(2024, 5, 20, 15, 17, 39, 37, DateTimeKind.Utc).AddTicks(4214) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2024, 5, 20, 15, 17, 38, 979, DateTimeKind.Utc).AddTicks(7649));
        }
    }
}
