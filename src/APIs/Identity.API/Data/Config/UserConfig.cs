using System;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.API.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();

            builder.HasIndex(m => m.AccountNumber).IsUnique();
            builder.Property(m => m.AccountNumber)
                .IsRequired()
                .HasMaxLength(50)
                .HasAnnotation("MinLength", 3);
            
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasAnnotation("MinLength", 3);

            builder.HasIndex(m => m.Username).IsUnique();
            
        }
    }
}
