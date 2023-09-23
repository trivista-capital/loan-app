using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifiedloanrequestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalWorkflow_LoanRequest_LoanRequestId",
                table: "ApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "LoanRequestId",
                table: "ApprovalWorkflow");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalWorkflowId",
                table: "LoanRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_ApprovalWorkflowId",
                table: "LoanRequest",
                column: "ApprovalWorkflowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRequest_ApprovalWorkflow_ApprovalWorkflowId",
                table: "LoanRequest",
                column: "ApprovalWorkflowId",
                principalTable: "ApprovalWorkflow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanRequest_ApprovalWorkflow_ApprovalWorkflowId",
                table: "LoanRequest");

            migrationBuilder.DropIndex(
                name: "IX_LoanRequest_ApprovalWorkflowId",
                table: "LoanRequest");

            migrationBuilder.DropColumn(
                name: "ApprovalWorkflowId",
                table: "LoanRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "LoanRequestId",
                table: "ApprovalWorkflow",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_LoanRequestId",
                table: "ApprovalWorkflow",
                column: "LoanRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalWorkflow_LoanRequest_LoanRequestId",
                table: "ApprovalWorkflow",
                column: "LoanRequestId",
                principalTable: "LoanRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
