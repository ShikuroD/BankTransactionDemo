using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Transaction.API.Models;
using Transaction.API.Data.Config;
namespace Transaction.API.Data
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
        {
        }

        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<AccountSummary> AccountSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AccountSummaryConfig());
            builder.ApplyConfiguration(new AccountTransactionConfig());
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
