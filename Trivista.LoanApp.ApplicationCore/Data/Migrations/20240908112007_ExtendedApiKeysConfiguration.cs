using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedApiKeysConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                table: "ClientApiKey",
                type: "nvarchar(3000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2024, 9, 8, 11, 20, 6, 187, DateTimeKind.Utc).AddTicks(6345), new DateTime(2024, 9, 8, 11, 20, 6, 187, DateTimeKind.Utc).AddTicks(6353) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2024, 9, 8, 11, 20, 6, 118, DateTimeKind.Utc).AddTicks(8271));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApiKey",
                table: "ClientApiKey",
                type: "nvarchar",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)");

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
    }
}
