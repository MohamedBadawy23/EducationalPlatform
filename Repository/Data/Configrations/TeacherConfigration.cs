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
    public class TeacherConfigration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasOne(T=>T.Center).WithMany().HasForeignKey(T=>T.CenterId).
                OnDelete(DeleteBehavior.Cascade);
            builder.Property(T => T.Name).IsRequired();

            builder.HasOne(T => T.Subject).WithMany().HasForeignKey(T => T.SubjectId).
             OnDelete(DeleteBehavior.Cascade);




        }
    }
}
