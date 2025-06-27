using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int CreditScore { get; set; }

        public Customer() { }

        public Customer(int customerID, string name, string emailAddress, string phoneNumber, string address, int creditScore)
        {
            CustomerID = customerID;
            Name = name;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            Address = address;
            CreditScore = creditScore;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Customer ID: {CustomerID}, Name: {Name}, Email: {EmailAddress}, Phone: {PhoneNumber}, Address: {Address}, Credit Score: {CreditScore}");
        }
    }
}
