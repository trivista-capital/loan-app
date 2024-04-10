using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLocationToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Customer",
                type: "nvarchar(3000)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 12, 14, 10, 6, 19, 516, DateTimeKind.Utc).AddTicks(5229), new DateTime(2023, 12, 14, 10, 6, 19, 516, DateTimeKind.Utc).AddTicks(5233) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                columns: new[] { "Created", "Location" },
                values: new object[] { new DateTime(2023, 12, 14, 10, 6, 19, 454, DateTimeKind.Utc).AddTicks(3307), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Customer");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 11, 21, 8, 34, 17, 70, DateTimeKind.Utc).AddTicks(1390), new DateTime(2023, 11, 21, 8, 34, 17, 70, DateTimeKind.Utc).AddTicks(1391) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2023, 11, 21, 8, 34, 17, 43, DateTimeKind.Utc).AddTicks(3687));
        }
    }
}
