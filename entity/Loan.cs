using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public class Loan
    {
        public int LoanId { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTerm { get; set; }
        public string LoanType { get; set; } // "HomeLoan" or "CarLoan"
        public string LoanStatus { get; set; } // "Pending" or "Approved"

        public Loan() { }

        public Loan(int loanId, Customer customer, decimal principalAmount, decimal interestRate, int loanTerm, string loanType, string loanStatus)
        {
            LoanId = loanId;
            Customer = customer;
            PrincipalAmount = principalAmount;
            InterestRate = interestRate;
            LoanTerm = loanTerm;
            LoanType = loanType;
            LoanStatus = loanStatus;
        }

        public virtual void PrintInfo()
        {
            Console.WriteLine($"Loan ID: {LoanId}, Customer: {Customer?.Name}, Amount: {PrincipalAmount}, Rate: {InterestRate}, Term: {LoanTerm}, Type: {LoanType}, Status: {LoanStatus}");
        }
    }
}
