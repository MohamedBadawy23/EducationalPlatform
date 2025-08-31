using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Core.Specefications
{
   public class UserByIdSpec:Specefication<AppUser>
    {
        public UserByIdSpec(string userId) : base(u => u.Id == userId)
        {
             
        }
    }
       
}
