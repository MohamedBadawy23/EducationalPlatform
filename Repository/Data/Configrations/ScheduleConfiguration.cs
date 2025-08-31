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
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasOne(P=>P.Subject).WithMany().HasForeignKey(P=>P.SubjectId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(P => P.Center).WithMany().HasForeignKey(P => P.CenterId).OnDelete(DeleteBehavior.Restrict); ;
            builder.HasOne(P => P.Teacher).WithMany().HasForeignKey(P => P.TeacherId).OnDelete(DeleteBehavior.Restrict); ;
            builder.HasMany(P => P.Students).WithMany();
            builder.HasOne(P => P.Room).WithMany().HasForeignKey(P => P.RoomId).OnDelete(DeleteBehavior.Restrict);


        }
    }
}
