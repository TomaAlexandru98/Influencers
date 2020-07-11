using Influencers.Models;
using Influencers.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Influencers.Repositories.Repositories
{
    public class EFBaseRepository<T> : IBaseRepository<T> where T : DataEntity
    {
        protected readonly BloggingContext dbContext;

        public EFBaseRepository(BloggingContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public T Add(T entity)
        {
            this.dbContext.Add<T>(entity);
            this.dbContext.SaveChanges();
            return entity;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.dbContext.Set<T>().AsEnumerable();
        }

        public virtual T GetById(int id)
        {
            return this.dbContext.Set<T>()
                                 .Where(entity => entity.Id == id)
                                 .SingleOrDefault();
        }

        public bool Remove(int id)
        {
            var entityToRemove = GetById(id);
            if (entityToRemove == null) return false;
            this.dbContext.Remove<T>(entityToRemove);
            this.dbContext.SaveChanges();
            return true;
        }

        public T Update(T entity)
        {
            this.dbContext.Update<T>(entity);
            this.dbContext.SaveChanges();
            return entity;
        }
    }
}
