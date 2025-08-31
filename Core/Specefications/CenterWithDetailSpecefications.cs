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
        public CenterWithDetailSpecefications():base()
        {
           
        }
        public CenterWithDetailSpecefications(Guid id):base(C=>C.Id==id)
        {
           
        }
    }
}
