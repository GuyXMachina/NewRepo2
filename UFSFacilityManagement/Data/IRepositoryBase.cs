﻿using UFSFacilityManagement.Data.DataAccess;
using UFSFacilityManagement.Models;
using System.Linq.Expressions;

namespace UFSFacilityManagement.Data
{
    public interface IRepositoryBase<T>
    {
        T GetById(string id);
        T GetById(int id);
        Task<List<T>> FindAllAsync();
        IEnumerable<T> FindAll();
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetWithOptions(QueryOptions<T> options);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
