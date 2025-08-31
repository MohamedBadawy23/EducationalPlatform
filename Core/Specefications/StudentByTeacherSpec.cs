using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class StudentByTeacherSpec:Specefication<Student>
    {
        public StudentByTeacherSpec(int teacherId)
        {
            
        }
    }
}
