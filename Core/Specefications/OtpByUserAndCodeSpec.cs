using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class OtpByUserAndCodeSpec:Specefication<OtpEntry>
    {
        public OtpByUserAndCodeSpec(string userId,string code):base(o => o.UserId == userId && o.Code == code)
        {
                
        }
    }
}
