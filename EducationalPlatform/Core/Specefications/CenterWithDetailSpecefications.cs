using Core.Entities;
using CoreProject.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;
 


namespace CoreProject.Specefications
{
   public class CenterWithDetailSpecefications:Specefication<Center>
    {
        public CenterWithDetailSpecefications()
        {
            
            //AddInclude(P => P.)
            //AddInclude(P => P.Students);
            //AddInclude(P => P.Subjects);
            //AddInclude(P => P.Lessons);
        }
        public CenterWithDetailSpecefications(int id):this()
        {
           
        }
    }
}
