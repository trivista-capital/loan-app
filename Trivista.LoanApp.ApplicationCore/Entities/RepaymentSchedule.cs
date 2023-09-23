using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class RepaymentSchedule: BaseEntity<Guid>
{
    private RepaymentSchedule() { }
    private RepaymentSchedule(Guid id, decimal amount, decimal repaymentAmount, RepaymentScheduleType repaymentType, ScheduleStatus status,
        Guid loanRequestId, DateTime dueDate, PaymentType paymentType)
    {
        Id = id;
        Amount = amount;
        RepaymentAmount = repaymentAmount;
        RepaymentType = repaymentType;
        Status = status;
        LoanRequestId = loanRequestId;
        DueDate = dueDate;
        PaymentType = paymentType;
        Created = DateTime.UtcNow;
        IsDue = false;
    }
    
    public decimal Amount { get; private set; }
    
    public decimal RepaymentAmount { get; private set; }
    
    public decimal LoanBalance { get; set; }
    
    public RepaymentScheduleType RepaymentType { get; private set; }
    
    public ScheduleStatus Status { get; private set; }
    
    public Guid LoanRequestId { get; private set; }
    
    public LoanRequest LoanRequest { get; set; }
    
    public DateTime DueDate { get; private set; }
    
    public PaymentType PaymentType { get; private set; }

    public Guid? TransactionId { get; private set; }

    public bool IsDue { get; set; }

    public ICollection<FailedPaymentAttempts> FailedPaymentAttempts { get; set; }

    public class Factory
    {
        public static List<RepaymentSchedule> GenerateLoanSchedule(decimal loanAmount, RepaymentScheduleType repaymentScheduleType, Guid loanRequestId, int loanTenure)
        {
            return CalculateMonthlyRepaymentSchedule(loanAmount, repaymentScheduleType,  loanRequestId, loanTenure);
        }
    }
    
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
    public RepaymentSchedule UpdateRepaymentStatus()
    {
        Status = ScheduleStatus.Paid;
        return this;
    }

    private static List<RepaymentSchedule> CalculateMonthlyRepaymentSchedule(decimal loanAmount, RepaymentScheduleType repaymentScheduleType, Guid loanRequestId, int loanTenure)
    {
        var repaymentSchedules = new List<RepaymentSchedule>();
        var repaymentAmountMonthly = loanAmount / loanTenure;
        if (repaymentScheduleType == RepaymentScheduleType.Monthly)
        {
            var initialDate = DateTime.UtcNow.AddDays(30);
            for (var i = 0; i < loanTenure; i++)
            {
                if (i == 0)
                {
                    var schedule =  new RepaymentSchedule(Guid.NewGuid(), loanAmount, repaymentAmountMonthly, RepaymentScheduleType.Monthly,
                        ScheduleStatus.Unpaid, loanRequestId, initialDate, PaymentType.Automatic);   
                    repaymentSchedules.Add(schedule);
                }
                else
                {
                    initialDate = initialDate.AddDays(30);
                    var schedule =  new RepaymentSchedule(Guid.NewGuid(), loanAmount, repaymentAmountMonthly, RepaymentScheduleType.Monthly,
                        ScheduleStatus.Unpaid, loanRequestId, initialDate, PaymentType.Automatic);   
                    repaymentSchedules.Add(schedule);
                }
            }

            return repaymentSchedules;
        }
        
        var repaymentAmountWeekly = repaymentAmountMonthly / 4;
        var initialWeekDate = DateTime.UtcNow.AddDays(7);
        var weeklyTenure = (4 * loanTenure);
        for (var i = 0; i < weeklyTenure; i++)
        {
            if (i == 0)
            {
                var schedule =  new RepaymentSchedule(Guid.NewGuid(), loanAmount, repaymentAmountWeekly, RepaymentScheduleType.Weekly,
                    ScheduleStatus.Unpaid,
                    loanRequestId, initialWeekDate, PaymentType.Automatic);
                repaymentSchedules.Add(schedule);   
            }
            else
            {
                initialWeekDate = initialWeekDate.AddDays(7);
                var schedule =  new RepaymentSchedule(Guid.NewGuid(), loanAmount, repaymentAmountWeekly, RepaymentScheduleType.Weekly,
                    ScheduleStatus.Unpaid,
                    loanRequestId, initialWeekDate, PaymentType.Automatic);
                repaymentSchedules.Add(schedule);
            }
        }

        return repaymentSchedules;
    }
}