using CoreProject.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specefications;
using Talabat.Repository;

namespace RepositoryProject.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EducationalPlatformDbContext _dbContext;

        public GenericRepository(EducationalPlatformDbContext dbContext)
        {
           _dbContext = dbContext;
        }
        public async Task AddAsync(T entity)
        {
             await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task<int> CountAsync(ISpecefications<T> Spec)=>
        
          await ApplySpecefication(Spec).CountAsync();
        

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecefications<T> Spec)
        {
            return await ApplySpecefication(Spec).ToListAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdWithSpecAsync(ISpecefications<T> Spec)
        {
            return await ApplySpecefication(Spec).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        private IQueryable<T> ApplySpecefication(ISpecefications<T> Spec)
        {
            return SpeceficationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);
        }

        
    }
}
