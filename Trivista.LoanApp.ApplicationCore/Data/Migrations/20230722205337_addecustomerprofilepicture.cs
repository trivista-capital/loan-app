using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addecustomerprofilepicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture_ProfilePictureFile",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePicture_ProfilePictureFileLength",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture_ProfilePictureFileName",
                table: "Customer",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture_ProfilePictureFileType",
                table: "Customer",
                type: "nvarchar(100)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerRemitterInformation",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ProfilePicture_ProfilePictureFile",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ProfilePicture_ProfilePictureFileLength",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ProfilePicture_ProfilePictureFileName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ProfilePicture_ProfilePictureFileType",
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
    }
}
