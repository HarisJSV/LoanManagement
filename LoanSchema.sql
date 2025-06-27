create database loan
use loan

CREATE TABLE CUSTOMER(
customerID INT PRIMARY KEY IDENTITY(1,1),
name nvarchar(30) NOT NULL,
emailAddress NVARCHAR(100) NOT NULL UNIQUE,
phoneNumber NVARCHAR(15),
address NVARCHAR(255),
creditScore INT 
);

CREATE TABLE LOAN(
loanid INT PRIMARY KEY IDENTITY(1,1),
customerid INT FOREIGN KEY REFERENCES CUSTOMER(customerID),
principalAmount DECIMAL(18,2) NOT NULL,
interestRate DECIMAL(5,2) NOT NULL,
loanTerm INT NOT NULL,
loanType NVARCHAR(20) CHECK (loanType IN ('HomeLoan', 'CarLoan')),
loanStatus NVARCHAR(20) CHECK (loanStatus IN ('Pending', 'Approved'))
);

CREATE TABLE HomeLoan (
    loanId INT PRIMARY KEY FOREIGN KEY REFERENCES Loan(loanId),
    propertyAddress NVARCHAR(255),
    propertyValue INT
);

CREATE TABLE CarLoan (
    loanId INT PRIMARY KEY FOREIGN KEY REFERENCES Loan(loanId),
    carModel NVARCHAR(100),
    carValue INT
);

INSERT INTO CUSTOMER (name, emailAddress, phoneNumber, address, creditScore)
VALUES 
('Haris M', 'haris@example.com', '9876543210', '123 Main St, Kochi', 720),
('Aisha R', 'aisha@example.com', '9123456789', '456 Park Ave, Chennai', 640),
('Rahul K', 'rahul@example.com', '9988776655', '789 Tech St, Bangalore', 800);

INSERT INTO LOAN (customerid, principalAmount, interestRate, loanTerm, loanType, loanStatus)
VALUES
(1, 500000.00, 7.5, 60, 'HomeLoan', 'Pending'),
(2, 300000.00, 9.2, 36, 'CarLoan', 'Pending'),
(3, 2500000.00, 6.8, 240, 'HomeLoan', 'Approved');

INSERT INTO HomeLoan (loanId, propertyAddress, propertyValue)
VALUES
(1, 'Plot #12, Green Valley, Kochi', 600000),
(3, 'Villa #22, Tech Park, Bangalore', 2800000);

INSERT INTO CarLoan (loanId, carModel, carValue)
VALUES
(2, 'Hyundai Creta 2022', 350000);

SELECT * FROM LOAN
SELECT * FROM CUSTOMER


ALTER TABLE LOAN
DROP CONSTRAINT CK__LOAN__loanStatus__3B75D760; -- Or whatever your constraint name is

ALTER TABLE LOAN
ADD CONSTRAINT CK_Loan_Status
CHECK (loanStatus IN ('Pending', 'Approved', 'Rejected'));
