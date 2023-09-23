using Shouldly;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.UnitTest.RepaymentSchedule;

public class RepaymentScheduleShould
{
    [Theory]
    [InlineData(2, "B21AC496-5CD2-4960-A8FC-83ABF8B41450")]
    public void Generate_5_Schedules_Given_Loan_Request_Exist_And_Payment_Type_Is_Monthly(int tenure, string loanRequestId)
    {
        //Arrange
        var firstRepaymentExpectedDate = DateTime.UtcNow.AddDays(30).Date;
        var secondRepaymentExpectedDate = firstRepaymentExpectedDate.AddDays(30).Date;
        var thirdRepaymentExpectedDate = secondRepaymentExpectedDate.AddDays(30).Date;
        var fourthRepaymentExpectedDate = thirdRepaymentExpectedDate.AddDays(30).Date;
        var fifthRepaymentExpectedDate = fourthRepaymentExpectedDate.AddDays(30).Date;
        
        var interest = Convert.ToDecimal((2.8 / 100) * 80000);
        var loanTotalRepaymentAmount = Loan.TotalRepaymentAmount(interest, 80000);
        
        //Act
        var reschedule = Entities.RepaymentSchedule.Factory.GenerateLoanSchedule(loanTotalRepaymentAmount, RepaymentScheduleType.Monthly,
            Guid.Parse(loanRequestId), tenure);
        
        //Assert
        if (reschedule.Count > tenure)
        {
            reschedule.OrderBy(x=>x.Created).Take(tenure);
        }
        
        reschedule.ShouldNotBe(null);
        reschedule.Count.ShouldBe(tenure);
        reschedule[0].RepaymentAmount.ShouldBe(200);
        reschedule[0].DueDate.Date.ShouldBe(firstRepaymentExpectedDate);
        reschedule[1].DueDate.Date.ShouldBe(secondRepaymentExpectedDate);
        reschedule[2].DueDate.Date.ShouldBe(thirdRepaymentExpectedDate);
        reschedule[3].DueDate.Date.ShouldBe(fourthRepaymentExpectedDate);
        reschedule[4].DueDate.Date.ShouldBe(fifthRepaymentExpectedDate);
        reschedule.ShouldAllBe(x=>x.Status == ScheduleStatus.Unpaid);
    }
    
    [Theory]
    [InlineData(1, "f10400bb-aa15-421f-a784-bba914cbff42")]
    public void Generate_5_Schedules_Given_Loan_Request_Exist_And_Payment_Type_Is_Weekly(int tenure, string loanRequestId)
    {
        //Arrange
        var loanAmount = 1000;
        var firstRepaymentExpectedDate = DateTime.UtcNow.AddDays(7).Date;
        var secondRepaymentExpectedDate = firstRepaymentExpectedDate.AddDays(7).Date;
        var thirdRepaymentExpectedDate = secondRepaymentExpectedDate.AddDays(7).Date;
        var fourthRepaymentExpectedDate = thirdRepaymentExpectedDate.AddDays(7).Date;
        var interest = Convert.ToDecimal((2.5 / 100) * loanAmount);
        var loanTotalRepaymentAmount = Loan.TotalRepaymentAmount(interest, loanAmount);
        
        //Act
        var reschedule = Entities.RepaymentSchedule.Factory.GenerateLoanSchedule(loanTotalRepaymentAmount, RepaymentScheduleType.Weekly, 
            Guid.Parse(loanRequestId), tenure);
        
        //Assert
        reschedule.ShouldNotBe(null);
        reschedule.Count.ShouldBe(4);
        //reschedule[0].RepaymentAmount.ShouldBe(55);
        reschedule[0].DueDate.Date.ShouldBe(firstRepaymentExpectedDate);
        reschedule[1].DueDate.Date.ShouldBe(secondRepaymentExpectedDate);
        reschedule[2].DueDate.Date.ShouldBe(thirdRepaymentExpectedDate);
        reschedule[3].DueDate.Date.ShouldBe(fourthRepaymentExpectedDate);
        reschedule.ShouldAllBe(x=>x.Status == ScheduleStatus.Unpaid);
        reschedule.ShouldAllBe(x=>x.RepaymentType == RepaymentScheduleType.Weekly);
    }
}