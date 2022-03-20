using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Database.Configuration
{
    public class MessagesDAOConfiguration : IEntityTypeConfiguration<MessageDAO>
    {
        public void Configure(EntityTypeBuilder<MessageDAO> builder)
        {
            builder
                .HasKey(x => x.Id)
                .HasName("PK_Messages");

            builder
                .ToTable("UserMassages")
                .HasOne<UserDAO>()
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .HasPrincipalKey(u => u.Id)
                .HasConstraintName("FK_User_Messages");

            builder
                .HasIndex(x => x.SentAt, "IDX_Messages_SentAt");
        }
    }
}
