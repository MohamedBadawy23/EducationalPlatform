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
    public class CenterConfigration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            builder.HasOne(U=>U.User).WithMany().HasForeignKey(U=>U.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(C => C.CenterName).IsRequired();
            builder.Property(C=>C.Address).IsRequired();
            builder.Property(C => C.Description).IsRequired().HasMaxLength(500);
            builder.Property(C => C.CreatedAt).IsRequired();
            builder.Property(C => C.PhoneNumber).IsRequired();


        }
    }
}
