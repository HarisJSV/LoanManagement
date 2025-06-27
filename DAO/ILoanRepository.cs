using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
namespace DAO
{
    public interface ILoanRepository
    {
        bool CreateCustomer(Customer customer);
        int LoginCustomer(string username, string password);
        bool ApplyLoan(Loan loan); // ask for user confirmation in implementation

        // b. Calculate Interest (by loanId)
        decimal CalculateInterest(int loanId);

        // b.i Overloaded: Calculate Interest (by direct params)
        decimal CalculateInterest(decimal principalAmount, decimal interestRate, int loanTerm);

        // c. Check and update Loan Status
        void LoanStatus(int loanId);

        // d. Calculate EMI (by loanId)
        decimal CalculateEMI(int loanId);

        // d.i Overloaded: Calculate EMI (by direct params)
        decimal CalculateEMI(decimal principalAmount, decimal interestRate, int loanTerm);

        // e. Repay Loan
        void LoanRepayment(int loanId, decimal amount);

        // f. Get all loans
        void GetAllLoans(int customerId);

        // g. Get loan by ID
        List<Loan> GetLoanById(int loanId);


    }
}
