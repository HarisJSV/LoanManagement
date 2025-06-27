namespace DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EmailAddress = c.String(),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        CreditScore = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        LoanId = c.Int(nullable: false, identity: true),
                        PrincipalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanTerm = c.Int(nullable: false),
                        LoanType = c.String(),
                        LoanStatus = c.String(),
                        CarModel = c.String(),
                        CarValue = c.Int(),
                        PropertyAddress = c.String(),
                        PropertyValue = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Customer_CustomerID = c.Int(),
                    })
                .PrimaryKey(t => t.LoanId)
                .ForeignKey("dbo.Customers", t => t.Customer_CustomerID)
                .Index(t => t.Customer_CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loans", "Customer_CustomerID", "dbo.Customers");
            DropIndex("dbo.Loans", new[] { "Customer_CustomerID" });
            DropTable("dbo.Loans");
            DropTable("dbo.Customers");
        }
    }
}
