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
            builder.HasOne(L => L.Lesson).WithMany().HasForeignKey(L => L.LessonId);
        }
    }
}
