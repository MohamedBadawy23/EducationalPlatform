using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class OtpByUserSpec:Specefication<OtpEntry>
    {
        public OtpByUserSpec(string userId) : base(o => o.UserId == userId)
        {
             
        }
    }
}
