using System;
using entity;
using DAO;
using exception;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement
{
    public class LoanManagement
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ILoanEfRepository efRepo = new LoanEfRepositoryImpl();
            LoanRepositoryImpl repo = new LoanRepositoryImpl();
            int loggedInCustId = -1;

            while (true)
            {
                Console.WriteLine("\n--- Welcome to Loan Management System ---");
                Console.WriteLine("1. Register (Customer)");
                Console.WriteLine("2. Login as Customer");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Enter name: ");
                        string name = Console.ReadLine();

                        Console.Write("Enter email: ");
                        string email = Console.ReadLine();

                        Console.Write("Enter phone number: ");
                        string phone = Console.ReadLine();

                        Console.Write("Enter address: ");
                        string address = Console.ReadLine();

                        Console.Write("Enter credit score: ");
                        int score = int.Parse(Console.ReadLine());

                        Customer newCustomer = new Customer
                        {
                            Name = name,
                            EmailAddress = email,
                            PhoneNumber = phone,
                            Address = address,
                            CreditScore = score
                        };

                        Console.WriteLine(repo.CreateCustomer(newCustomer) ? "Customer registered!" : "Registration failed.");
                        break;


                    case "2":
                        try
                        {
                            Console.Write("Enter email: ");
                            string emailaddress = Console.ReadLine();

                            Console.Write("Enter phone number: ");
                            string phoneno = Console.ReadLine();

                            loggedInCustId = repo.LoginCustomer(emailaddress, phoneno);
                            Console.WriteLine("Customer logged in successfully!");
                            ShowCustomerMenu(repo, efRepo,loggedInCustId); // Just like ShowUserMenu
                        }
                        catch (CustomerNotFoundException e)
                        {
                            Console.WriteLine("Login failed: " + e.Message);
                        }
                        break;


                    case "0":
                        Console.WriteLine("Exited.");
                        return;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            }
        }

        static void ShowCustomerMenu(ILoanRepository repo, ILoanEfRepository efRepo,int customerId)
        {
            while (true)
            {
                Console.WriteLine("\n--- Customer Menu ---");
                Console.WriteLine("1. Apply for Loan");              
                Console.WriteLine("2. Calculate Interest");            
                Console.WriteLine("3. View Loan Status");             
                Console.WriteLine("4. Calculate EMI");                 
                Console.WriteLine("5. Repay Loan");                    
                Console.WriteLine("6. View My Loans");                 
                Console.WriteLine("7. View Loan By ID");               
                Console.WriteLine("8. View My Loans usinf Entity Framework.");
                Console.WriteLine("0. Logout");
                Console.Write("Choose an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Write("Enter loan type (HomeLoan/CarLoan): ");
                        string loanType = Console.ReadLine();

                        Console.Write("Enter principal amount: ");
                        decimal principal = decimal.Parse(Console.ReadLine());

                        Console.Write("Enter interest rate (annual %): ");
                        decimal interest = decimal.Parse(Console.ReadLine());

                        Console.Write("Enter loan term (months): ");
                        int term = int.Parse(Console.ReadLine());

                        Loan loan = new Loan
                        {
                            Customer = new Customer { CustomerID = customerId },
                            PrincipalAmount = principal,
                            InterestRate = interest,
                            LoanTerm = term,
                            LoanType = loanType,
                            LoanStatus = "Pending" // default status
                        };

                        Console.WriteLine(repo.ApplyLoan(loan)
                            ? "Loan application submitted successfully!"
                            : "Loan application failed.");
                        break;

                     case "2":
                        try
                        {
                            Console.Write("Enter Loan ID to calculate interest: ");
                            int loanId = int.Parse(Console.ReadLine());

                            interest = repo.CalculateInterest(loanId);
                            Console.WriteLine($"Total interest amount: ₹{interest:F2}");
                        }
                        catch (InvalidLoanException e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                        break;

                    case "3":
                        Console.Write("Enter Loan ID to check status: ");
                        int loanID = int.Parse(Console.ReadLine());

                        try
                        {
                            repo.LoanStatus(loanID);
                        }
                        catch (InvalidLoanException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;

                    case "4":
                        try
                        {
                            Console.Write("Enter Loan ID to calculate EMI: ");
                            int emiLoanId = int.Parse(Console.ReadLine());

                            decimal emi = repo.CalculateEMI(emiLoanId);
                            Console.WriteLine($"Estimated Monthly EMI: ₹{emi}");
                        }
                        catch (InvalidLoanException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number.");
                        }
                        break;

                    case "5":
                        try
                        {
                            Console.Write("Enter Loan ID to repay: ");
                            int repayLoanId = int.Parse(Console.ReadLine());

                            Console.Write("Enter amount to repay: ");
                            decimal repayAmount = decimal.Parse(Console.ReadLine());

                            repo.LoanRepayment(repayLoanId, repayAmount);
                        }
                        catch (InvalidLoanException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input. Please enter valid numbers.");
                        }
                        break;


                    case "6":
                        try
                        {
                            repo.GetAllLoans(customerId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An unexpected error occurred: " + ex.Message);
                        }
                        break;


                    case "7":
                        try
                        {
                            Console.Write("Enter Customer ID to view loans: ");
                            int custId = int.Parse(Console.ReadLine());

                            List<Loan> loans = repo.GetLoanById(custId);
                            //NOT TO SELF: DO NOT FORGET TO REMOVE THE USER ID CONSOLE LINE COZ THE FUNCTION ALREADY PASSES IT. IMPORRRTANNNNT
                            if (loans.Count == 0)
                            {
                                throw new CustomerNotFoundException($"No loans found for Customer ID: {custId}");
                            }

                            Console.WriteLine($"--- Loans for Customer ID {custId} ---");
                            foreach (var loani in loans)
                            {
                                Console.WriteLine($"Loan ID: {loani.LoanId}, Type: {loani.LoanType}, Amount: {loani.PrincipalAmount}, Term: {loani.LoanTerm}, Status: {loani.LoanStatus}");
                            }
                        }
                        catch (CustomerNotFoundException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An unexpected error occurred: " + ex.Message);
                        }
                        break;


                    case "8":
                        Console.WriteLine("Fetching all loans using Entity Framework...");
                        try
                        {
                            var efloans = efRepo.GetLoansByCustomerId(customerId);

                            if (efloans == null || efloans.Count == 0)
                            {
                                Console.WriteLine("No loans found in the database.");
                            }
                            else
                            {
                                foreach (var i in efloans)
                                {
                                    Console.WriteLine($"Loan ID: {i.LoanId}, Type: {i.LoanType}, Amount: ₹{i.PrincipalAmount}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("EF Retrieval failed: " + ex.Message);
                        }
                        break;


                    case "0":
                        Console.WriteLine("Logging out...");
                        return;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            }
        }


    }
}
