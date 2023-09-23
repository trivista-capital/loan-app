using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedproofofaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProofOfAddress_ProofOFAddressFile",
                table: "LoanRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProofOfAddress_ProofOFAddressFileLength",
                table: "LoanRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfAddress_ProofOFAddressFileName",
                table: "LoanRequest",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProofOfAddress_ProofOFAddressFileType",
                table: "LoanRequest",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofOfAddress_ProofOFAddressFile",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "ProofOfAddress_ProofOFAddressFileLength",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "ProofOfAddress_ProofOFAddressFileName",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "ProofOfAddress_ProofOFAddressFileType",
                table: "LoanRequest");

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
    }
}
