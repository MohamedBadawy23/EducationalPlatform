using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class TeacherWithSubjectAndStudents:Specefication<Teacher>
    {
        public TeacherWithSubjectAndStudents(Guid centerId): base(t => t.CenterId == centerId)
        {
            AddInclude(t => t.Subject);
            AddInclude(t => t.Students);
        }
    }
}
