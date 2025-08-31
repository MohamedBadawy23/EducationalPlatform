using Core.Entities;
using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class StudentByIdSpec:Specefication<Student>
    {
        public StudentByIdSpec():base()
        {
                
        }
        public StudentByIdSpec(int stuId,Guid centerId):base(P=>P.Id==stuId&&P.CenterId==centerId)
        {
            AddInclude(P => P.Subjects);
           // AddInclude($"{nameof(Student.Subjects)}.{nameof(Subject.Teachers)}");


        }
        public StudentByIdSpec(string Userid):base(P => P.UserId == Userid)
        {
            AddInclude(P => P.User  );
        }
    }
}
