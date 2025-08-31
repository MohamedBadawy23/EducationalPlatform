using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class MessageByCenterSpec:Specefication<Message>
    {
        public MessageByCenterSpec():base()
        {
            
        }
        public MessageByCenterSpec(Guid centerId):base(P=>P.Teacher.CenterId==centerId)
        {
            AddInclude(P=>P.Teacher);
            AddInclude(P=>P.Students);
        }
        public MessageByCenterSpec(Guid centerId,int messageId) : base(P => P.Teacher.CenterId == centerId&&P.Id==messageId)
        {
            AddInclude(P => P.Teacher);
            AddInclude(P => P.Students);
        }
    }
}
