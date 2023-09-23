using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addmbsandindicinaproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankStatementAnalysis",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MbsBankStatement",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 8, 11, 19, 53, 13, 318, DateTimeKind.Utc).AddTicks(3650), new DateTime(2023, 8, 11, 19, 53, 13, 318, DateTimeKind.Utc).AddTicks(3650) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                columns: new[] { "BankStatementAnalysis", "Created", "MbsBankStatement", "UserType" },
                values: new object[] { null, new DateTime(2023, 8, 11, 19, 53, 13, 289, DateTimeKind.Utc).AddTicks(1550), null, "Staff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankStatementAnalysis",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "MbsBankStatement",
                table: "Customer");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 8, 2, 4, 21, 1, 513, DateTimeKind.Utc).AddTicks(1100), new DateTime(2023, 8, 2, 4, 21, 1, 513, DateTimeKind.Utc).AddTicks(1100) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                columns: new[] { "Created", "UserType" },
                values: new object[] { new DateTime(2023, 8, 2, 4, 21, 1, 477, DateTimeKind.Utc).AddTicks(8870), null });
        }
    }
}
