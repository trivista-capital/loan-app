using Microsoft.AspNetCore.Components.Web;
using Trivista.LoanApp.ApplicationCore.DomainEvents;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class Loan: BaseEntity<int>
{
    internal Loan(){}

    private Loan(string name, decimal interestRate, decimal maximumLoanAmount, decimal maximumTenure, decimal minimumSalary, bool isDefault)
    {
        Name = name;
        InterestRate = interestRate;
        MaximumLoanAmount = maximumLoanAmount;
        MaximumTenure = maximumTenure;
        MinimumSalary = minimumSalary;
        IsDefault = isDefault;
    }
    public string Name { get; private set; }
    
    public bool IsDefault { get; private set; }
    public decimal InterestRate { get; set; }
    public decimal MaximumLoanAmount{ get; set;}
    
    public decimal MinimumSalary{ get; set;}
    public decimal MaximumTenure{ get; set;}
    
    public static decimal ActualCalculableSalary(decimal monthlySalary, decimal averageMonthlyRepayment)
    {
        var actualCalculableSalary = monthlySalary - averageMonthlyRepayment;
        return actualCalculableSalary;
    } 
    
    public static decimal RepayableMonthlyIncome(decimal actualCalculableSalary)
    {
        var repayableMonthlyIncome = (65.0m / 100) * actualCalculableSalary;
        return repayableMonthlyIncome;
    } 
    
    public static decimal MonthlyRepaymentAmount_Interest(decimal interestRate, decimal loanAmount)
    {
        var monthlyRepaymentAmount_Interest = ((interestRate / 100) * loanAmount);
        return monthlyRepaymentAmount_Interest;
    } 
    
    public static decimal TotalRepaymentAmount_Interest(decimal interestRate, decimal loanAmount, int tenure)
    {
        var totalRepaymentAmount_Interest = ((interestRate / 100) * loanAmount * tenure);
        return totalRepaymentAmount_Interest;
    } 
    
    public static decimal MonthlyRepaymentAmount_Principal(decimal loanAmount, int tenure)
    {
        var monthlyRepaymentAmount_Principal = loanAmount / tenure;
        return monthlyRepaymentAmount_Principal;
    }

    public static decimal MonthlyRepaymentAmount(decimal monthlyPayableInterest, decimal monthlyPayablePrincipalAmount)
    {
        var monthlyRepaymentAmount = monthlyPayableInterest + monthlyPayablePrincipalAmount;
        return monthlyRepaymentAmount;
    } 
    
    public static decimal TotalRepaymentAmount(decimal totalPayableInterest, decimal totalPayablePrincipalAmount)
    {
        var totalRepaymentAmount = totalPayableInterest + totalPayablePrincipalAmount;
        return totalRepaymentAmount;
    }

    protected override void When(object @event)
    {
        switch (@event)
        {
            case LoanEvents.LoanCreated e:
                Id = e.Id;
                Name = e.Name;
                break;
            case LoanEvents.LoanDeleted e:
                Id = e.Id;
                Name = e.Name;
                break;
        }
    }
    
    public Loan SetName(string name)
    {
        this.Name = name;
        return this;
    }
    
    public Loan SetInterestRate(decimal interestRate)
    {
        this.InterestRate = interestRate;
        return this;
    }
    
    public Loan SetMinimumSalary(decimal minimumSalary)
    {
        this.MinimumSalary = minimumSalary;
        return this;
    }
    
    public Loan SetMaximumLoanAmount(decimal maximumLoanAmount)
    {
        this.MaximumLoanAmount = maximumLoanAmount;
        return this;
    }
    
    public Loan SetMaximumTenure(decimal maximumTenure)
    {
        this.MaximumTenure = maximumTenure;
        return this;
    }

    public Loan SetDefaultLoan(bool isDefault)
    {
        this.IsDefault = isDefault;
        return this;
    }
    
    public class Factory
    {
        public static Loan Build()
        {
            return new Loan();
        }
        
        public static Loan Create(string name, decimal interestRate, decimal maximumLoanAmount, decimal maximumTenure, decimal minimumSalary, bool isDefault)
        {
            return new Loan(name, interestRate, maximumLoanAmount, maximumTenure, minimumSalary, isDefault);
        }
    }
}