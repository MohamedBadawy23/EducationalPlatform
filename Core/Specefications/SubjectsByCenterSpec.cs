using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class SubjectsByCenterSpec:Specefication<Subject>
    {
        public SubjectsByCenterSpec():base()
        {
                
        }
        public SubjectsByCenterSpec(int id,Guid centerId) : base(P=>P.Id==id &&P.CenterId==centerId)
        {
            
        }
        public SubjectsByCenterSpec(  Guid centerId) : base( P=> P.CenterId == centerId)
        {

        }
    }
}
