using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
 

namespace Talabat.Core.Specefications
{
    public class Specefication<T> : ISpecefications<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get  ; set  ; }

        public List<Expression<Func<T, object>>> Includes { get ; set ; }=/*السطر ده بدل اللي تحت علشان امنع تكرار الكود*/ new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get ; set  ; }
         
        public Expression<Func<T, object>> OrderByDescending { get  ; set ; }
        public int Take { get  ; set ; }
        public int Skip { get  ; set  ; }
        public bool IsPaginationEnabled { get  ; set  ; }
        public List<string> IncludeString { get  ; set  ; }=new();

        public Specefication()
        {
            //Includes = new List<Expression<Func<T,object>>>();]
        }
        public Specefication(Expression<Func<T,bool>>CriteriaExprsssion)
        {
            Criteria = CriteriaExprsssion;
            //Includes = new List<Expression<Func<T, object>>>();
        }
        public void AddOrderBy(Expression<Func<T, object>> OrderByExprssion)
        {
            OrderBy = OrderByExprssion;
        }
        public void AddOrderByDescending(Expression<Func<T, object>> OrderByDescendingExprssion)
        {
             OrderByDescending = OrderByDescendingExprssion;
        }
        public void AddInclude(Expression<Func<T, object>> IncludeExpression)
        {
            Includes.Add(IncludeExpression);
        }
        public void AddInclude(string IncludeString1)
        {
           IncludeString.Add(IncludeString1);
        }

    }
}
