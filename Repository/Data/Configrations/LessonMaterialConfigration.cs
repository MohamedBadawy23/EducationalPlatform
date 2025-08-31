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
    public class LessonMaterialConfigration : IEntityTypeConfiguration<LessonMaterial>
    {
        public void Configure(EntityTypeBuilder<LessonMaterial> builder)
        {
            builder.Property(P => P.Title).IsRequired();
            builder.Property(P => P.Description).IsRequired();
            builder.Property(P => P.FilePath).IsRequired();




            builder.HasOne(L => L.Lesson).WithMany().HasForeignKey(L => L.LessonId);
            builder.HasOne(P=>P.Center).WithMany().HasForeignKey(P=>P.CenterId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(P => P.Teacher).WithMany().HasForeignKey(P => P.TeacherId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(P => P.Subject).WithMany().HasForeignKey(P => P.SubjectId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}
