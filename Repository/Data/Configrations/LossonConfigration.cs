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
    public class LossonConfigration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasOne(L=>L.Subject).WithMany().HasForeignKey(L=>L.SubjectId);
            builder.Property(L=>L.StartTime).IsRequired();
            builder.Property(L => L.EndTime).IsRequired();
            builder.Property(L => L.Title).IsRequired();


        }
    }
}
