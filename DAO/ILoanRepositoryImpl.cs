using entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using exception;
using System.Threading.Tasks;
using Utils;

namespace DAO
{
    public class LoanRepositoryImpl : ILoanRepository
    {

        public int LoginCustomer(string email, string phone)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = "SELECT customerID FROM CUSTOMER WHERE emailAddress = @Email AND phoneNumber = @Phone";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);

                con.Open();
                var result = cmd.ExecuteScalar();

                if (result == null)
                {
                    throw new CustomerNotFoundException("Invalid email or phone number.");
                }

                return Convert.ToInt32(result);
            }
        }
        public bool CreateCustomer(Customer customer)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = "INSERT INTO CUSTOMER (name, emailAddress, phoneNumber, address, creditScore) " +
                               "VALUES (@name, @email, @phone, @address, @score)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", customer.Name);
                cmd.Parameters.AddWithValue("@email", customer.EmailAddress);
                cmd.Parameters.AddWithValue("@phone", customer.PhoneNumber);
                cmd.Parameters.AddWithValue("@address", customer.Address);
                cmd.Parameters.AddWithValue("@score", customer.CreditScore);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
        public bool ApplyLoan(Loan loan)
        {
            decimal interest = CalculateInterest(loan.PrincipalAmount, loan.InterestRate, loan.LoanTerm);
            decimal emi = CalculateEMI(loan.PrincipalAmount, loan.InterestRate, loan.LoanTerm);
            Console.WriteLine($"Expected Interest Amount: ₹{interest}");
            Console.WriteLine($"Estimated Monthly EMI: ₹{emi}");
            Console.WriteLine("Do you want to apply for the loan? (Yes/No): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToLower() != "yes")
            {
                Console.WriteLine("Loan application cancelled.");
                return false;
            }

            try
            {
                using (SqlConnection conn = DBUtil.GetConnection())
                {
                    conn.Open();

                    string insertLoanQuery = @"INSERT INTO Loan (customerId, principalAmount, interestRate, loanTerm, loanType, loanStatus) 
                                           VALUES (@customerId, @principalAmount, @interestRate, @loanTerm, @loanType, @loanStatus); 
                                           SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(insertLoanQuery, conn);
                    cmd.Parameters.AddWithValue("@customerId", loan.Customer.CustomerID);
                    cmd.Parameters.AddWithValue("@principalAmount", loan.PrincipalAmount);
                    cmd.Parameters.AddWithValue("@interestRate", loan.InterestRate);
                    cmd.Parameters.AddWithValue("@loanTerm", loan.LoanTerm);
                    cmd.Parameters.AddWithValue("@loanType", loan.LoanType);
                    cmd.Parameters.AddWithValue("@loanStatus", "Pending");

                    int loanId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (loan is HomeLoan homeLoan)
                    {
                        string insertHome = "INSERT INTO HomeLoan (loanId, propertyAddress, propertyValue) VALUES (@loanId, @address, @value)";
                        SqlCommand cmdHome = new SqlCommand(insertHome, conn);
                        cmdHome.Parameters.AddWithValue("@loanId", loanId);
                        cmdHome.Parameters.AddWithValue("@address", homeLoan.PropertyAddress);
                        cmdHome.Parameters.AddWithValue("@value", homeLoan.PropertyValue);
                        cmdHome.ExecuteNonQuery();
                    }
                    else if (loan is CarLoan carLoan)
                    {
                        string insertCar = "INSERT INTO CarLoan (loanId, carModel, carValue) VALUES (@loanId, @model, @value)";
                        SqlCommand cmdCar = new SqlCommand(insertCar, conn);
                        cmdCar.Parameters.AddWithValue("@loanId", loanId);
                        cmdCar.Parameters.AddWithValue("@model", carLoan.CarModel);
                        cmdCar.Parameters.AddWithValue("@value", carLoan.CarValue);
                        cmdCar.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while applying for loan: {ex.Message}");
                return false;
            }
        }

        public decimal CalculateInterest(int loanId)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = "SELECT PrincipalAmount, InterestRate, LoanTerm FROM Loan WHERE LoanId = @loanId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@loanId", loanId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidLoanException("Loan not found for ID: " + loanId);
                }

                decimal principal = reader.GetDecimal(0);
                decimal rate = reader.GetDecimal(1);
                int term = reader.GetInt32(2);

                return CalculateInterest(principal, rate, term); // use overload
            }
        }

        public decimal CalculateInterest(decimal principal, decimal rate, int tenure)
        {
            return (principal * rate * tenure) / 12 / 100; // convert rate to percentage
        }

        public void LoanStatus(int loanId)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = @"SELECT L.LoanId, C.CreditScore
                         FROM Loan L
                         JOIN Customer C ON L.CustomerId = C.CustomerId
                         WHERE L.LoanId = @loanId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@loanId", loanId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidLoanException("Loan not found for ID: " + loanId);
                }

                int creditScore = reader.GetInt32(1);
                reader.Close();

                string status = creditScore > 650 ? "approved" : "rejected";

                string updateQuery = "UPDATE Loan SET LoanStatus = @status WHERE LoanId = @loanId";
                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@status", status);
                updateCmd.Parameters.AddWithValue("@loanId", loanId);

                int rows = updateCmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine($"Loan ID {loanId} has been {status} based on credit score.");
                }
                else
                {
                    Console.WriteLine("Failed to update loan status.");
                }
            }
        }

        public decimal CalculateEMI(int loanId)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = "SELECT PrincipalAmount, InterestRate, LoanTerm FROM Loan WHERE LoanId = @loanId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@loanId", loanId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidLoanException("Loan not found for ID: " + loanId);
                }

                decimal principal = reader.GetDecimal(0);
                decimal annualRate = reader.GetDecimal(1);
                int tenure = reader.GetInt32(2);

                return CalculateEMI(principal, annualRate, tenure); // use overload
            }
        }

        // Overload to calculate EMI when applying for a loan
        public decimal CalculateEMI(decimal principal, decimal annualRate, int tenure)
        {
            decimal monthlyRate = annualRate / 12 / 100; // R
            int N = tenure; // N in months

            if (monthlyRate == 0)
                return principal / N; // No interest loan

            decimal numerator = principal * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, N);
            decimal denominator = (decimal)Math.Pow(1 + (double)monthlyRate, N) - 1;

            return Math.Round(numerator / denominator, 2); // Round off to 2 decimal places
        }


        public void LoanRepayment(int loanId, decimal amount)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string selectQuery = "SELECT PrincipalAmount, InterestRate, LoanTerm FROM Loan WHERE LoanId = @loanId";
                SqlCommand cmd = new SqlCommand(selectQuery, con);
                cmd.Parameters.AddWithValue("@loanId", loanId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidLoanException("Loan not found for ID: " + loanId);
                }

                decimal principal = reader.GetDecimal(0);
                decimal rate = reader.GetDecimal(1);
                int tenure = reader.GetInt32(2);
                reader.Close();

                decimal emi = CalculateEMI(principal, rate, tenure);

                if (amount < emi)
                {
                    Console.WriteLine($"Amount ₹{amount} is less than the monthly EMI ₹{emi}. Payment rejected.");
                    return;
                }

                int emiCount = (int)(amount / emi);

                Console.WriteLine($"You have paid {emiCount} EMI(s) for Loan ID {loanId} with ₹{amount}.");
                Console.WriteLine($"(EMI per month: ₹{emi:F2})");
            }
        }


        public void GetAllLoans(int customerId)
        {
            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = "SELECT LoanId, PrincipalAmount, InterestRate, LoanTerm, LoanType, LoanStatus FROM Loan WHERE CustomerId = @customerId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("No loans found for this customer.");
                    return;
                }

                Console.WriteLine("\n--- Loan Details ---");
                while (reader.Read())
                {
                    Console.WriteLine($"Loan ID      : {reader.GetInt32(0)}");
                    Console.WriteLine($"Principal    : ₹{reader.GetDecimal(1)}");
                    Console.WriteLine($"InterestRate : {reader.GetDecimal(2)}%");
                    Console.WriteLine($"Loan Term    : {reader.GetInt32(3)} months");
                    Console.WriteLine($"Loan Type    : {reader.GetString(4)}");
                    Console.WriteLine($"Status       : {reader.GetString(5)}");
                    Console.WriteLine("------------------------------");
                }
            }
        }

        public List<Loan> GetLoanById(int customerId)
        {
            List<Loan> loans = new List<Loan>();

            using (SqlConnection con = DBUtil.GetConnection())
            {
                string query = @"SELECT LoanId, PrincipalAmount, InterestRate, LoanTerm, LoanType, LoanStatus 
                         FROM Loan WHERE CustomerId = @cid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    loans.Add(new Loan
                    {
                        LoanId = reader.GetInt32(0),
                        PrincipalAmount = reader.GetDecimal(1),
                        InterestRate = reader.GetDecimal(2),
                        LoanTerm = reader.GetInt32(3),
                        LoanType = reader.GetString(4),
                        LoanStatus = reader.GetString(5),
                        Customer = new Customer { CustomerID = customerId } // basic info
                    });
                }
            }

            return loans;
        }

    }

}
