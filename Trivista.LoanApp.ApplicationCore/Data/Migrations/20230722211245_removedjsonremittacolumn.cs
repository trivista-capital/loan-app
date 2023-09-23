using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedjsonremittacolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation",
                table: "Customer");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_AverageSixMonthsSalary",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_IsRemittaUser",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation_OtherLoansCollected",
                table: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "CustomerRemitterInformation",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
