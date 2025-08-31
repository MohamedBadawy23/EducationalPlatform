using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configrations
{
    public class MessageConfigration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(U => U.AppUser).WithMany()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(M => M.SentDate).IsRequired();
            builder.Property(L => L.Content).IsRequired();
            builder.Property(L => L.IsRead).IsRequired();

        }
    }
}
