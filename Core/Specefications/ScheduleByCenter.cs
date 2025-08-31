using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class ScheduleByCenter:Specefication<Schedule>
    {
        public ScheduleByCenter(Guid centerId)
        {
                Criteria=P=>P.CenterId==centerId;

            AddInclude(P => P.Subject);
            AddInclude(P => P.Room);
           // AddInclude(P => P.Subject.Teachers);

        }
    }
}
