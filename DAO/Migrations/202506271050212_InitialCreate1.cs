namespace DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Loans", "Customer_CustomerID", "dbo.Customers");
            DropIndex("dbo.Loans", new[] { "Customer_CustomerID" });
            RenameColumn(table: "dbo.Loans", name: "Customer_CustomerID", newName: "CustomerId");
            AlterColumn("dbo.Loans", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Loans", "CustomerId");
            AddForeignKey("dbo.Loans", "CustomerId", "dbo.Customers", "CustomerID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loans", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Loans", new[] { "CustomerId" });
            AlterColumn("dbo.Loans", "CustomerId", c => c.Int());
            RenameColumn(table: "dbo.Loans", name: "CustomerId", newName: "Customer_CustomerID");
            CreateIndex("dbo.Loans", "Customer_CustomerID");
            AddForeignKey("dbo.Loans", "Customer_CustomerID", "dbo.Customers", "CustomerID");
        }
    }
}
