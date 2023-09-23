using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addeddefaultadminuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 8, 1, 21, 8, 56, 675, DateTimeKind.Utc).AddTicks(3920), new DateTime(2023, 8, 1, 21, 8, 56, 675, DateTimeKind.Utc).AddTicks(3920) });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "Address", "Bvn", "City", "Country", "Created", "Deleted", "DeletedBy", "Dob", "Email", "FirstName", "IsDeleted", "LastModifiedBy", "LastName", "LoaneeTypes", "MbsRequestStatementResponseCode", "MiddleName", "Occupation", "PhoneNumber", "PostCode", "RoleId", "Sex", "State" },
                values: new object[] { new Guid("363b37a0-c306-4472-a405-4b576334cca0"), null, null, null, null, new DateTime(2023, 8, 1, 21, 8, 56, 638, DateTimeKind.Utc).AddTicks(1370), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "", "femi.ibitolu@gmail.com", "Babafemi", false, new Guid("00000000-0000-0000-0000-000000000000"), "Ibitolu", 0, 0, null, null, "", null, "3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d", "Male", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("363b37a0-c306-4472-a405-4b576334cca0"));

            migrationBuilder.UpdateData(
                table: "ApplicationRole",
                keyColumn: "Id",
                keyValue: new Guid("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d"),
                columns: new[] { "CreatedOn", "LastModified" },
                values: new object[] { new DateTime(2023, 8, 1, 9, 28, 25, 481, DateTimeKind.Utc).AddTicks(2050), new DateTime(2023, 8, 1, 9, 28, 25, 481, DateTimeKind.Utc).AddTicks(2050) });
        }
    }
}
