using DAO;
using entity;
using exception;
using NUnit.Framework;
using System.Data.SqlClient;
using Utils;
namespace LoanTests
{
    public class Tests
    {
        private ILoanRepository _repo;
        [SetUp]
        public void Setup()
        {
            _repo = new LoanRepositoryImpl();
        }

        [Test]
        public void CalculateEMI_ShouldReturnCorrectValue()
        {

            decimal principal = 200000m;       
            decimal annualRate = 10m;         
            int loanTerm = 24;                 

            decimal actualEmi = _repo.CalculateEMI(principal, annualRate, loanTerm);
            decimal expectedEmi = 9228.99m;
            Assert.That(actualEmi, Is.EqualTo(expectedEmi).Within(0.1m));
        }

        [Test]
        public void CalculateEMI_InvalidLoanId_ShouldThrowException()
        {
            Assert.Throws<InvalidLoanException>(() => _repo.CalculateEMI(-99));
        }

        [Test]
        public void LoanStatus_ShouldThrowException_ForInvalidLoan()
        {
            Assert.Throws<InvalidLoanException>(() => _repo.LoanStatus(-1));
        }

        [Test]
        public void LoanRepayment_LessThanEMI_ShouldRejectPayment()
        {
            int loanId = 1;
            decimal smallAmount = 10;
            Assert.DoesNotThrow(() => _repo.LoanRepayment(loanId, smallAmount));
        }
    }

}