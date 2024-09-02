using Coravel.Invocable;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Data.Context;

namespace Trivista.LoanApp.ApplicationCore.Features.BackgroundServices
{
    public class SendMailHostedService: IInvocable
    {
        private readonly TrivistaDbContext _trivistaDbContext;
        private readonly IPublisher _publisher;
        private readonly ILogger<SendMailHostedService> _logger;

        public SendMailHostedService(
            TrivistaDbContext trivistaDbContext,
            IPublisher publisher,
            ILogger<SendMailHostedService> logger)
        {
            _trivistaDbContext = trivistaDbContext;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Invoke()
        {
            try
            {
                var loanRequests = await _trivistaDbContext.
                    LoanRequest.
                    AsNoTracking().
                Include(x => x.RepaymentSchedules.Where(x => !x.IsDue)).
                Include(x => x.Customer).
                Where(x => x.LoanApplicationStatus == Commons.Enums.LoanApplicationStatus.Active).
                GroupBy(x => x.Customer.Email).
                Select(x => new
                {
                    Email = x.Key,
                    tenure = x.SelectMany(x => x.RepaymentSchedules).Count(),
                    Schedules = x.SelectMany(c => c.RepaymentSchedules).ToList(),
                    Customer = x.Where(o => o.Customer.Email == x.Key).Select(o => o.Customer).FirstOrDefault()
                }).
                ToListAsync(CancellationToken.None);

                var today = DateTime.UtcNow.Date;

                foreach (var loan in loanRequests)
                {
                    var OneDayFromNow = today.AddDays(1);
                    var dueLoannOneDay = loan.Schedules.Where(x => x.DueDate.Date == OneDayFromNow).ToList();
                    foreach (var item in dueLoannOneDay)
                    {
                        await _publisher.Publish(new DueLoanEvent()
                        {
                            To = loan.Customer!.Email,
                            CustomerName = $"{loan.Customer!.FirstName} {loan.Customer!.MiddleName} {loan!.Customer!.LastName}",
                            ApprovedLoanAmount = item.Amount,
                            ApprovedLoanTenure = loan.tenure,
                            RepaymentAmount = item.RepaymentAmount,
                            DueDate = item.DueDate
                        });
                    }


                    var threeDaysFromNow = today.AddDays(3);
                    var dueLoannThreeDays = loan.Schedules.Where(x => x.DueDate.Date == threeDaysFromNow).ToList();
                    foreach (var item in dueLoannThreeDays)
                    {
                        await _publisher.Publish(new DueLoanEvent()
                        {
                            To = loan.Customer!.Email,
                            CustomerName = $"{loan.Customer!.FirstName} {loan.Customer!.MiddleName} {loan!.Customer!.LastName}",
                            ApprovedLoanAmount = item.Amount,
                            ApprovedLoanTenure = loan.tenure,
                            RepaymentAmount = item.RepaymentAmount,
                            DueDate = item.DueDate
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while looping through loan request for notice");
                throw;
            }
        }
    }
}
