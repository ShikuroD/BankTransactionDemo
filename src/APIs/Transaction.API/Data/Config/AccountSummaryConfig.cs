using System;
using Transaction.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Transaction.API.Data.Config
{
    public class AccountSummaryConfig: IEntityTypeConfiguration<AccountSummary>
    {
        public void Configure(EntityTypeBuilder<AccountSummary> builder)
        {
            builder.HasKey(m => m.AccountNumber);
            builder.Property(m => m.AccountNumber).ValueGeneratedOnAdd();

            builder.HasMany<AccountTransaction>(m => m.AccountTransactions)
                .WithOne(a => a.AccountSummary)
                .HasForeignKey(a => a.AccountNumber);

            builder.Property(m => m.Balance)
                .HasColumnType("decimal(19,2)");
        }

    }
}
