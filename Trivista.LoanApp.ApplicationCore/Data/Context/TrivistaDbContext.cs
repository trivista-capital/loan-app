using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.Context;

public class TrivistaDbContext: DbContext
{
    public TrivistaDbContext(DbContextOptions<TrivistaDbContext> options) : base(options) { }
    
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Loan> Loan { get; set; }
    public DbSet<LoanRequest> LoanRequest { get; set; }
    
    public DbSet<DisbursementApproval> DisbursementApproval { get; set; }
    public DbSet<ApprovalWorkflowConfiguration> ApprovalWorkflowConfiguration { get; set; }
    public DbSet<ApprovalWorkflowApplicationRoleConfiguration> ApprovalWorkflowApplicationRoleConfiguration { get; set; }
    
    public DbSet<ApprovalWorkflow> ApprovalWorkflow { get; set; }
    
    public DbSet<ApprovalWorkflowApplicationRole> ApprovalWorkflowApplicationRole { get; set; }
    public DbSet<ApplicationRole> ApplicationRole { get; set; }
    public DbSet<RepaymentSchedule> RepaymentSchedule { get; set; }
    
    public DbSet<FailedPaymentAttempts> FailedPaymentAttempts { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<Ticket> Ticket { get; set; }
    public DbSet<TicketMessages> TicketMessage { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrivistaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}