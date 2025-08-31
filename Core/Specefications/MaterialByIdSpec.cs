using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class MaterialByIdSpec:Specefication<LessonMaterial>
    {
        public MaterialByIdSpec():base()
        {
            
        }
        public MaterialByIdSpec(Guid cenetrId,int MaterialId):base(P=>P.Id==MaterialId&&P.CenterId==cenetrId)
        {
            AddInclude(P => P.Subject);

            //AddInclude(P=>P.Subject.Teachers);
        }
    }
}
