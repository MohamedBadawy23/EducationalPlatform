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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(P => P.Enrollment)
                .WithMany( )
                .HasForeignKey(P => P.EnrlollmentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(P => P.User).WithMany().HasForeignKey(P => P.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(P => P.Subject).WithMany().HasForeignKey(P => P.SubjectId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
