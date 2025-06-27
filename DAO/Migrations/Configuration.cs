namespace DAO.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DAO.LoanDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DAO.LoanDbContext";
        }

        protected override void Seed(DAO.LoanDbContext context)
        {
        }
    }
}
