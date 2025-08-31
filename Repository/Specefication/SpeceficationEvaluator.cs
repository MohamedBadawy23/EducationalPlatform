 using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace Talabat.Repository
{
   public static class SpeceficationEvaluator<T> where T :class
    {
        public static IQueryable<T>GetQuery(IQueryable<T> inputQuery,ISpecefications<T> Spec)
        {
            var Query = inputQuery;
            if(Spec.Criteria is not null)
            {
                Query = Query.Where(Spec.Criteria);
            }
            if(Spec.OrderBy is not null)
            {
              Query=  Query.OrderBy(Spec.OrderBy);
            }
            if(Spec.OrderByDescending is not null)
            {
                Query=Query.OrderByDescending(Spec.OrderByDescending);
            }
             
            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            Query = Spec.IncludeString.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            return Query;
        }

    }
}
