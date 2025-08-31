using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class ScheduleByIdSpec:Specefication<Schedule>
    {
        public ScheduleByIdSpec():base()
        {
            
        }
        public ScheduleByIdSpec(int schuId,Guid centerId) : base(P=>P.Id==schuId&&P.CenterId==centerId)
        {
            AddInclude(P => P.Subject);
            AddInclude(P => P.Teacher);
            AddInclude(P => P.Room);


        }

    }
}
