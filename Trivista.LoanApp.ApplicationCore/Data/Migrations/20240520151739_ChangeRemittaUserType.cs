using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRemittaUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "Customer",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

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
                columns: new[] { "Created", "Email", "FirstName", "LastName" },
                values: new object[] { new DateTime(2024, 5, 20, 15, 17, 38, 979, DateTimeKind.Utc).AddTicks(7649), "tgslimited@gmail.com", "Admin", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "Customer",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 12, 14, 10, 28, 3, 872, DateTimeKind.Utc).AddTicks(6082), new DateTime(2023, 12, 14, 10, 28, 3, 872, DateTimeKind.Utc).AddTicks(6086) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                columns: new[] { "Created", "Email", "FirstName", "LastName" },
                values: new object[] { new DateTime(2023, 12, 14, 10, 28, 3, 812, DateTimeKind.Utc).AddTicks(8980), "femi.ibitolu@gmail.com", "Babafemi", "Ibitolu" });
        }
    }
}
