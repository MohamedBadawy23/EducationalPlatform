using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class StudentsByCenterSpec:Specefication<Student>
    {
        public StudentsByCenterSpec():base()
        {
                
        }
        //public StudentsByCenterSpec(int id) : base(P=>P.Id==id)
        //{

        //}
        public StudentsByCenterSpec(Guid centerId,int studentId) : base(P => P.CenterId == centerId&&P.Id==studentId)
        {

        }
        public StudentsByCenterSpec(Guid centerId)
        {
            Criteria = P => P.CenterId == centerId;
            AddInclude(P => P.Subjects);
            AddInclude("Subjects.Teacher");
        }

    }
}
