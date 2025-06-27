using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exception
{
    public class InvalidLoanException : Exception
    {
        public InvalidLoanException() : base("Loan ID not found.") { }
        public InvalidLoanException(string message) : base(message) { }
    }
}
