using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dob = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaneeTypes = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumTenure = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bvn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoanApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    DisbursedLoanStatus = table.Column<int>(type: "int", nullable: false),
                    kycDetailsCustomerFirstName = table.Column<string>(name: "kycDetails_CustomerFirstName", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerMiddleName = table.Column<string>(name: "kycDetails_CustomerMiddleName", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerLastName = table.Column<string>(name: "kycDetails_CustomerLastName", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerEmail = table.Column<string>(name: "kycDetails_CustomerEmail", type: "nvarchar(300)", nullable: false),
                    kycDetailsCustomerAddress = table.Column<string>(name: "kycDetails_CustomerAddress", type: "nvarchar(300)", nullable: false),
                    kycDetailsCustomerCity = table.Column<string>(name: "kycDetails_CustomerCity", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerState = table.Column<string>(name: "kycDetails_CustomerState", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerCountry = table.Column<string>(name: "kycDetails_CustomerCountry", type: "nvarchar(50)", nullable: false),
                    kycDetailsCustomerPostalCode = table.Column<string>(name: "kycDetails_CustomerPostalCode", type: "nvarchar(100)", nullable: false),
                    kycDetailsCustomerOccupation = table.Column<string>(name: "kycDetails_CustomerOccupation", type: "nvarchar(300)", nullable: false),
                    kycDetailsCustomerPhoneNumber = table.Column<string>(name: "kycDetails_CustomerPhoneNumber", type: "nvarchar(300)", nullable: false),
                    SalaryDetailsAverageMonthlyNetSalary = table.Column<decimal>(name: "SalaryDetails_AverageMonthlyNetSalary", type: "decimal(18,2)", nullable: false),
                    SalaryDetailsSalaryAccountNumber = table.Column<string>(name: "SalaryDetails_SalaryAccountNumber", type: "nvarchar(12)", nullable: false),
                    SalaryDetailsBankName = table.Column<string>(name: "SalaryDetails_BankName", type: "nvarchar(80)", nullable: false),
                    SalaryDetailsAccountName = table.Column<string>(name: "SalaryDetails_AccountName", type: "nvarchar(70)", nullable: false),
                    SalaryDetailsAccountStatementFileName = table.Column<string>(name: "SalaryDetails_AccountStatementFileName", type: "nvarchar(max)", nullable: false),
                    SalaryDetailsAccountStatementFileType = table.Column<string>(name: "SalaryDetails_AccountStatementFileType", type: "nvarchar(max)", nullable: false),
                    SalaryDetailsAccountStatementFileLength = table.Column<long>(name: "SalaryDetails_AccountStatementFileLength", type: "bigint", nullable: false),
                    SalaryDetailsAccountStatementFile = table.Column<byte[]>(name: "SalaryDetails_AccountStatementFile", type: "varbinary(max)", nullable: false),
                    LoanDetailsLoanAmount = table.Column<decimal>(name: "LoanDetails_LoanAmount", type: "decimal(18,2)", nullable: false),
                    LoanDetailstenure = table.Column<int>(name: "LoanDetails_tenure", type: "int", nullable: false),
                    LoanDetailspurpose = table.Column<string>(name: "LoanDetails_purpose", type: "nvarchar(400)", nullable: false),
                    LoanDetailsRepaymentScheduleType = table.Column<decimal>(name: "LoanDetails_RepaymentScheduleType", type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerRemitterInformationIsRemittaUser = table.Column<bool>(name: "CustomerRemitterInformation_IsRemittaUser", type: "bit", nullable: false),
                    CustomerRemitterInformationAverageSixMonthsSalary = table.Column<decimal>(name: "CustomerRemitterInformation_AverageSixMonthsSalary", type: "decimal(18,2)", nullable: false),
                    CustomerRemitterInformationOtherLoansCollected = table.Column<decimal>(name: "CustomerRemitterInformation_OtherLoansCollected", type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_CustomerId",
                table: "LoanRequest",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loan");

            migrationBuilder.DropTable(
                name: "LoanRequest");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
