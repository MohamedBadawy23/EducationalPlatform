using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class SubjectByName:Specefication<Subject>
    {
        public SubjectByName(string subjectName,Guid centerId):base(P=>P.Name==subjectName&&P.CenterId==centerId)
        {
            
        }
    }
}
