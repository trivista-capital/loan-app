using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedsalarydetailsfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_AverageSixMonthsSalary",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_OtherLoansCollected",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_AccountStatementFile",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_AccountStatementFileLength",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_AccountStatementFileName",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_AccountStatementFileType",
                table: "LoanRequest");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerRemitterInformation_AverageSixMonthsSalary",
                table: "Customer",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerRemitterInformation_OtherLoansCollected",
                table: "Customer",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow",
                column: "LoanRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_AverageSixMonthsSalary",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_OtherLoansCollected",
                table: "Customer");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerRemitterInformation_AverageSixMonthsSalary",
                table: "LoanRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "LoanRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerRemitterInformation_OtherLoansCollected",
                table: "LoanRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte[]>(
                name: "SalaryDetails_AccountStatementFile",
                table: "LoanRequest",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "SalaryDetails_AccountStatementFileLength",
                table: "LoanRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SalaryDetails_AccountStatementFileName",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryDetails_AccountStatementFileType",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow",
                column: "LoanRequestId");
        }
    }
}
