using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeddatatypeofrejectedbyfromguidtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RejectedBy",
                table: "ApprovalWorkflowApplicationRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "ApprovalWorkflowApplicationRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "ApprovalWorkflow",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "ApprovalWorkflow",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "ApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "ApprovalWorkflow");

            migrationBuilder.AlterColumn<Guid>(
                name: "RejectedBy",
                table: "ApprovalWorkflowApplicationRole",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedBy",
                table: "ApprovalWorkflowApplicationRole",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 8, 16, 20, 5, 50, 334, DateTimeKind.Utc).AddTicks(5610), new DateTime(2023, 8, 16, 20, 5, 50, 334, DateTimeKind.Utc).AddTicks(5610) });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"),
                column: "Created",
                value: new DateTime(2023, 8, 16, 20, 5, 50, 301, DateTimeKind.Utc).AddTicks(6550));
        }
    }
}
