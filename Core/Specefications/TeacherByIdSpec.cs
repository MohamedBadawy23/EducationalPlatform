using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class TeacherByIdSpec:Specefication<Teacher>
    {
        public TeacherByIdSpec():base()
        {
            
        }
        public TeacherByIdSpec(int teacherId, Guid centerId) :base(P=>P.Id==teacherId&&P.CenterId==centerId) 
        {
            AddInclude(P => P.Subject);
        }

    }
}
