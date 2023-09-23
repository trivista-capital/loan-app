using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedefaultadminrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ApplicationRole",
                type: "nvarchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApplicationRole",
                type: "nvarchar(400)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "ApplicationRole",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DeletedOn", "Description", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[] { new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"), null, new DateTime(2023, 7, 30, 2, 56, 25, 963, DateTimeKind.Utc).AddTicks(9940), null, "Default super admin role", false, new DateTime(2023, 7, 30, 2, 56, 25, 963, DateTimeKind.Utc).AddTicks(9940), null, "SuperAdmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ApplicationRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApplicationRole",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldNullable: true);
        }
    }
}
