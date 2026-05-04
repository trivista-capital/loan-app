using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions.EventHandlers;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.BackgroundServices
{
    public class DueLoanEvent: INotification
    {
        public string To { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public decimal ApprovedLoanAmount { get; set; } = default!;
        public int ApprovedLoanTenure { get; set; } = default!;
        public decimal RepaymentAmount { get; set; } = default!;
        public DateTime DueDate { get; set; } = default!;
    }

    public class DueLoanEventHandler : INotificationHandler<DueLoanEvent>
    {
        private readonly IMailService _mailManager;

        public DueLoanEventHandler(IMailService mailManager)
        {
            _mailManager = mailManager;
        }

        public async Task Handle(DueLoanEvent notification, CancellationToken cancellationToken)
        {
            _mailManager.BuildLoanDueMessage(notification.To,
                                                       notification.CustomerName,
                                                       notification.ApprovedLoanAmount,
                                                       notification.ApprovedLoanTenure,
                                                       notification.RepaymentAmount,
                                                       notification.DueDate);
        }
    }
}
