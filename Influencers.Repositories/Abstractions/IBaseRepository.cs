using Influencers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.Repositories.Abstractions
{
    public interface IBaseRepository<T> where T : DataEntity
    {
        T Add(T entity);
        T Update(T entity);
        bool Remove(int id);
        T GetById(int id);
        IEnumerable<T> GetAll();
    }
}
