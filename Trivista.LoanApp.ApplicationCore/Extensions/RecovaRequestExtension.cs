using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Features.Dto;

namespace Trivista.LoanApp.ApplicationCore.Extensions
{
    public static class RecovaRequestExtension
    {
        public static RecovaRequest ToRecovaRequest(
            this Customer customer, 
            LoanRequest request, 
            string bankCode,
            List<Guarantor> guarantors)
        {
            return new RecovaRequest()
            {
                Bvn = customer.Bvn,
                BusinessRegistrationNumber = "",
                TaxIdentificationNumber = "",
                LoanReference = request.Id.ToString(),
                CustomerID = customer.Id.ToString(),
                CustomerName = $"{customer.FirstName} {customer.MiddleName} {customer.LastName}",
                CustomerEmail = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                LoanAmount = request.LoanDetails.LoanAmount,
                TotalRepaymentExpected = request.RepaymentSchedules.Sum(x => x.Amount),
                LoanTenure = request.RepaymentSchedules.Count,
                LinkedAccountNumber = request.SalaryDetails.SalaryAccountNumber,
                PreferredRepaymentBankCBNCode = bankCode,
                PreferredRepaymentAccount = request.SalaryDetails.SalaryAccountNumber,
                CollectionPaymentSchedules = request.RepaymentSchedules.Select(x => new CollectionPaymentSchedule()
                {
                    RepaymentAmountInNaira = x.Amount,
                    RepaymentDate = x.DueDate.ToShortDateString()
                }).ToList(),
                Guarantors = guarantors,
            };
        }
    }
}
