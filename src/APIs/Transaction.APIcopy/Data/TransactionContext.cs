using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Transaction.APIcopy.Models;
using Transaction.APIcopy.Data.Config;
namespace Transaction.APIcopy.Data
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
