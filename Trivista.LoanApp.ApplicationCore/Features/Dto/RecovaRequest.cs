using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivista.LoanApp.ApplicationCore.Features.Dto
{
    public class RecovaRequest
    {
        public string Bvn { get; set; } = default!;
        public string BusinessRegistrationNumber { get; set; } = default!;
        public string TaxIdentificationNumber { get; set; } = default!;
        public string LoanReference { get; set; } = default!;
        public string CustomerID { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public decimal LoanAmount { get; set; } = default!;
        public decimal TotalRepaymentExpected { get; set; } = default!;
        public int LoanTenure { get; set; } = default!;
        public string LinkedAccountNumber { get; set; } = default!;
        public string RepaymentType { get; set; } = "collection"!;
        public string PreferredRepaymentBankCBNCode { get; set; } = default!;
        public string PreferredRepaymentAccount { get; set; } = default!;
        public List<CollectionPaymentSchedule> CollectionPaymentSchedules { get; set; } =
            new List<CollectionPaymentSchedule>();
    }

    public class RecovaResponse
    {
        public int Id { get; set; }
        public string Bvn { get; set; } = default!;
        public string BusinessRegistrationNumber { get; set; } = default!;
        public string TaxIdentificationNumber { get; set; } = default!;
        public string LoanReference { get; set; } = default!;
        public string CustomerID { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string LoanAmount { get; set; } = default!;
        public string TotalRepaymentExpected { get; set; } = default!;
        public string LoanTenure { get; set; } = default!;
        public string LinkedAccountNumber { get; set; } = default!;
        public string RequestStatus { get; set; } = default!;
        public string RepaymentType { get; set; } = "Recovery"!;
        public string PreferredRepaymentBankCBNCode { get; set; } = default!;
        public string PreferredRepaymentAccount { get; set; } = default!;
        public string ConsentApprovalUrl { get; set; } = default!;
    }

    public class CollectionPaymentSchedule
    {
        public string RepaymentDate { get; set; } = default!;
        public decimal RepaymentAmountInNaira { get; set; } = 0;
    }

    public class Guarantor
    {
        public string Bvn { get; set; } = default!;
        public int GuaranteeingPercentage { get; set; } = 0;
    }
}
