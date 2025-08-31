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
    public class LessonBookingConfigration : IEntityTypeConfiguration<LessonBooking>
    {
        public void Configure(EntityTypeBuilder<LessonBooking> builder)
        {
            builder.HasOne(L=>L.Lesson).WithMany().HasForeignKey(L=>L.LessonId);
            builder.Property(L => L.isPaid).IsRequired();
            builder.Property(L => L.BookingDate).IsRequired();


        }
    }
}
