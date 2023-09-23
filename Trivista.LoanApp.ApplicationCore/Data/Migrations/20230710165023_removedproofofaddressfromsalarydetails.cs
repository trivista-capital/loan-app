using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedproofofaddressfromsalarydetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryDetails_ProofOFAddressFile",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_ProofOFAddressFileLength",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_ProofOFAddressFileName",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "SalaryDetails_ProofOFAddressFileType",
                table: "LoanRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalaryDetails_ProofOFAddressFile",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SalaryDetails_ProofOFAddressFileLength",
                table: "LoanRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SalaryDetails_ProofOFAddressFileName",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryDetails_ProofOFAddressFileType",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
