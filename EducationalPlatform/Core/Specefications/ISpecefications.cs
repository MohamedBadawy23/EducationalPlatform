using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
 

namespace Talabat.Core.Specefications
{
    public interface ISpecefications<T> where T :class
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        //Ascending
        public Expression<Func<T, object>>OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
         

    }
}
