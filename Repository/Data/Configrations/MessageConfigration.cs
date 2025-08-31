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
            builder.HasMany(P => P.Students).WithMany();
            builder.HasOne(P => P.Center).WithMany().HasForeignKey(P => P.CenterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(P => P.Teacher).WithMany().HasForeignKey(P => P.TeacherId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(M => M.SentDate).IsRequired();
            builder.Property(L => L.Title).IsRequired();
            builder.Property(L => L.IsRead).IsRequired();

        }
    }
}
