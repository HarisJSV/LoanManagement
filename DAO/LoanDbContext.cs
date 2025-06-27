using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using entity;

namespace DAO
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext() : base("name=LoanDbContext") { }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Add more DbSet<> properties if needed
    }
}
