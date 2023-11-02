using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedFailedRemitaDisbursementclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedRemitaDisbursement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsReProcessed = table.Column<bool>(type: "bit", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedRemitaDisbursement", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 11, 2, 13, 41, 7, 470, DateTimeKind.Utc).AddTicks(3141), new DateTime(2023, 11, 2, 13, 41, 7, 470, DateTimeKind.Utc).AddTicks(3145) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2023, 11, 2, 13, 41, 7, 417, DateTimeKind.Utc).AddTicks(5835));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedRemitaDisbursement");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 10, 28, 22, 22, 17, 582, DateTimeKind.Utc).AddTicks(1092), new DateTime(2023, 10, 28, 22, 22, 17, 582, DateTimeKind.Utc).AddTicks(1097) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2023, 10, 28, 22, 22, 17, 545, DateTimeKind.Utc).AddTicks(3548));
        }
    }
}
