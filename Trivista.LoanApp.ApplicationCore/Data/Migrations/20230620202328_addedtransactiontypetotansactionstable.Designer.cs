﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trivista.LoanApp.ApplicationCore.Data.Context;

#nullable disable

namespace Trivista.LoanApp.ApplicationCore.Data.Migrations
{
    [DbContext(typeof(TrivistaDbContext))]
    [Migration("20230620202328_addedtransactiontypetotansactionstable")]
    partial class addedtransactiontypetotansactionstable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationRole");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApprovalWorkflowConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateApproved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRejected")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalWorkflowConfigurationId");

                    b.ToTable("ApprovalWorkflow");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApprovalWorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApprovedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateApproved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRejected")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Hierarchy")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RejectedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalWorkflowId");

                    b.ToTable("ApprovalWorkflowApplicationRole");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowApplicationRoleConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ApprovalWorkflowConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("CanOverrideAllApprovals")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Hierarchy")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalWorkflowConfigurationId");

                    b.ToTable("ApprovalWorkflowApplicationRoleConfiguration");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ApprovalWorkflowConfiguration");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Bvn")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("City")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Dob")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("LoaneeTypes")
                        .HasColumnType("int");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Occupation")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PostCode")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Sex")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("State")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.FailedPaymentAttempts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RepaymentScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.HasIndex("RepaymentScheduleId");

                    b.ToTable("FailedPaymentAttempts");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Loan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("MaximumLoanAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("MaximumTenure")
                        .HasColumnType("int");

                    b.Property<decimal>("MinimumSalary")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Loan");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.LoanRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApprovalWorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Bvn")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DisbursedLoanStatus")
                        .HasColumnType("int");

                    b.Property<decimal>("Interest")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LoanApplicationStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalWorkflowId")
                        .IsUnique();

                    b.HasIndex("CustomerId");

                    b.ToTable("LoanRequest");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.RepaymentSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("DateTime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDue")
                        .HasColumnType("bit");

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("LoanBalance")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<Guid>("LoanRequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PaymentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("RepaymentAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("RepaymentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LoanRequestId");

                    b.ToTable("RepaymentSchedule");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deleted")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuccessful")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("LastModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("nvarchar");

                    b.Property<Guid>("RepaymentScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TransactionReference")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RepaymentScheduleId")
                        .IsUnique();

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflow", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowConfiguration", "ApprovalWorkflowConfiguration")
                        .WithMany()
                        .HasForeignKey("ApprovalWorkflowConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApprovalWorkflowConfiguration");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowApplicationRole", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflow", "ApprovalWorkflow")
                        .WithMany("ApprovalWorkflowApplicationRole")
                        .HasForeignKey("ApprovalWorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApprovalWorkflow");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowApplicationRoleConfiguration", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowConfiguration", "ApprovalWorkflowConfiguration")
                        .WithMany("Roles")
                        .HasForeignKey("ApprovalWorkflowConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApprovalWorkflowConfiguration");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Customer", b =>
                {
                    b.OwnsOne("Trivista.LoanApp.ApplicationCore.Entities.ValueObjects.CustomerRemitterInformation", "CustomerRemitterInformation", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("AverageSixMonthsSalary")
                                .HasColumnType("decimal(18, 2)");

                            b1.Property<bool>("IsRemittaUser")
                                .HasColumnType("bit");

                            b1.Property<decimal>("OtherLoansCollected")
                                .HasColumnType("decimal(18, 2)");

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.Navigation("CustomerRemitterInformation")
                        .IsRequired();
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.FailedPaymentAttempts", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.RepaymentSchedule", "RepaymentSchedule")
                        .WithMany("FailedPaymentAttempts")
                        .HasForeignKey("RepaymentScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RepaymentSchedule");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.LoanRequest", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflow", "ApprovalWorkflow")
                        .WithOne()
                        .HasForeignKey("Trivista.LoanApp.ApplicationCore.Entities.LoanRequest", "ApprovalWorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.Customer", "Customer")
                        .WithMany("LoanRequests")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Trivista.LoanApp.ApplicationCore.Entities.ValueObjects.LoanDetails", "LoanDetails", b1 =>
                        {
                            b1.Property<Guid>("LoanRequestId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("LoanAmount")
                                .HasColumnType("decimal(18, 2)");

                            b1.Property<decimal>("LoanBalance")
                                .HasColumnType("decimal(18, 2)");

                            b1.Property<decimal>("RepaymentScheduleType")
                                .HasColumnType("decimal(18, 2)");

                            b1.Property<string>("purpose")
                                .IsRequired()
                                .HasColumnType("nvarchar(400)");

                            b1.Property<int>("tenure")
                                .HasColumnType("int");

                            b1.HasKey("LoanRequestId");

                            b1.ToTable("LoanRequest");

                            b1.WithOwner()
                                .HasForeignKey("LoanRequestId");
                        });

                    b.OwnsOne("Trivista.LoanApp.ApplicationCore.Entities.ValueObjects.SalaryDetails", "SalaryDetails", b1 =>
                        {
                            b1.Property<Guid>("LoanRequestId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("AccountName")
                                .IsRequired()
                                .HasColumnType("nvarchar(70)");

                            b1.Property<decimal>("AverageMonthlyNetSalary")
                                .HasColumnType("decimal(18, 2)");

                            b1.Property<string>("BankName")
                                .IsRequired()
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("SalaryAccountNumber")
                                .IsRequired()
                                .HasColumnType("nvarchar(12)");

                            b1.HasKey("LoanRequestId");

                            b1.ToTable("LoanRequest");

                            b1.WithOwner()
                                .HasForeignKey("LoanRequestId");
                        });

                    b.OwnsOne("Trivista.LoanApp.ApplicationCore.Entities.ValueObjects.kycDetails", "kycDetails", b1 =>
                        {
                            b1.Property<Guid>("LoanRequestId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CustomerAddress")
                                .IsRequired()
                                .HasColumnType("nvarchar(300)");

                            b1.Property<string>("CustomerCity")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("CustomerCountry")
                                .IsRequired()
                                .HasColumnType("nvarchar(50)");

                            b1.Property<string>("CustomerEmail")
                                .IsRequired()
                                .HasColumnType("nvarchar(300)");

                            b1.Property<string>("CustomerFirstName")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("CustomerLastName")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("CustomerMiddleName")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("CustomerOccupation")
                                .IsRequired()
                                .HasColumnType("nvarchar(300)");

                            b1.Property<string>("CustomerPhoneNumber")
                                .IsRequired()
                                .HasColumnType("nvarchar(300)");

                            b1.Property<string>("CustomerPostalCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("CustomerState")
                                .IsRequired()
                                .HasColumnType("nvarchar(100)");

                            b1.HasKey("LoanRequestId");

                            b1.ToTable("LoanRequest");

                            b1.WithOwner()
                                .HasForeignKey("LoanRequestId");
                        });

                    b.Navigation("ApprovalWorkflow");

                    b.Navigation("Customer");

                    b.Navigation("LoanDetails")
                        .IsRequired();

                    b.Navigation("SalaryDetails")
                        .IsRequired();

                    b.Navigation("kycDetails")
                        .IsRequired();
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.RepaymentSchedule", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.LoanRequest", "LoanRequest")
                        .WithMany("RepaymentSchedules")
                        .HasForeignKey("LoanRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoanRequest");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Transaction", b =>
                {
                    b.HasOne("Trivista.LoanApp.ApplicationCore.Entities.RepaymentSchedule", "RepaymentSchedule")
                        .WithOne("Transaction")
                        .HasForeignKey("Trivista.LoanApp.ApplicationCore.Entities.Transaction", "RepaymentScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RepaymentSchedule");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflow", b =>
                {
                    b.Navigation("ApprovalWorkflowApplicationRole");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowConfiguration", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.Customer", b =>
                {
                    b.Navigation("LoanRequests");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.LoanRequest", b =>
                {
                    b.Navigation("RepaymentSchedules");
                });

            modelBuilder.Entity("Trivista.LoanApp.ApplicationCore.Entities.RepaymentSchedule", b =>
                {
                    b.Navigation("FailedPaymentAttempts");

                    b.Navigation("Transaction");
                });
#pragma warning restore 612, 618
        }
    }
}
