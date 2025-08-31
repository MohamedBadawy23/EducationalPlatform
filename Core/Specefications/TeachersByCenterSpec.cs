using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class TeachersByCenterSpec:Specefication<Teacher>
    {
        public TeachersByCenterSpec():base()
        {
                
        }
        public TeachersByCenterSpec(Guid centerId) : base(P=>P.CenterId==centerId)
        {
                AddInclude(p=>p.Subject);
        }
        
    }
}
