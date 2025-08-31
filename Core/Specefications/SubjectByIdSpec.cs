using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class SubjectByIdSpec:Specefication<Subject>
    {
        public SubjectByIdSpec():base()
        {
            
        }
        public SubjectByIdSpec(int id) : base(P=>P.Id==id)
        {

        }
        public SubjectByIdSpec(string subjectName) : base(P=>P.Name==subjectName )
        {

        }


        //public SubjectByIdSpec(Guid centerId,List<int> subId):base(P=>P.CenterId==centerId&&subId.Contains(P.Id))
        //{
        //   // AddInclude(P => P.Teachers);
        //}
    }
}
