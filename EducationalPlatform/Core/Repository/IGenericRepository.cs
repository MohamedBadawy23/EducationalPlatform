using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;

namespace CoreProject.Repository
{
   public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecefications<T> Spec);
        Task<T>GetByIdWithSpecAsync(ISpecefications<T> Spec);
        Task<int> CountAsync(ISpecefications<T> Spec);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangeAsync();
    }
}
