using System.Transactions;
using System;
using Transaction.APIcopy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Transaction.APIcopy.Data.Config
{
    public class AccountTransactionConfig : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
            builder.HasKey(m => m.TransactionId);
            builder.Property(m => m.TransactionId).ValueGeneratedOnAdd();

            builder.Property(m => m.Amount)
                .HasColumnType("decimal(19,2)");

            
        }

    }
}
