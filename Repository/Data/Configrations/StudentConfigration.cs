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
    public class StudentConfigration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(S => S.Center).WithMany().HasForeignKey(S => S.CenterId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(S => S.Subjects).WithMany();
            builder.HasOne(S => S.Teacher).WithMany().HasForeignKey(S => S.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(S=>S.FullName).IsRequired();
            builder.Property(S => S.EnrollmentDate).IsRequired();
            builder.Property(C => C.PhoneNumber).IsRequired();


        }
    }
}
