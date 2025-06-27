using DAO;
using entity;
using System.Collections.Generic;
using System.Linq;

public class LoanEfRepositoryImpl : ILoanEfRepository
{
    public List<Loan> GetLoansByCustomerId(int customerId)
    {
        using (var context = new LoanDbContext())
        {
            return context.Loans
                          .Where(l => l.CustomerId == customerId)
                          .ToList();
        }
    }

}

