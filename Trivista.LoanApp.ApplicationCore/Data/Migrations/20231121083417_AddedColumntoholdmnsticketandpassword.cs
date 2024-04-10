using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumntoholdmnsticketandpassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MbsBankStatementTicketAndPassword",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

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
                columns: new[] { "Created", "MbsBankStatementTicketAndPassword" },
                values: new object[] { new DateTime(2023, 11, 21, 8, 34, 17, 43, DateTimeKind.Utc).AddTicks(3687), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MbsBankStatementTicketAndPassword",
                table: "Customer");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 11, 9, 19, 43, 2, 981, DateTimeKind.Utc).AddTicks(792), new DateTime(2023, 11, 9, 19, 43, 2, 981, DateTimeKind.Utc).AddTicks(794) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2023, 11, 9, 19, 43, 2, 946, DateTimeKind.Utc).AddTicks(7270));
        }
    }
}
