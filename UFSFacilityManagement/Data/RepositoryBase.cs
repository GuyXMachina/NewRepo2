﻿using UFSFacilityManagement.Data.DataAccess;
using UFSFacilityManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace UFSFacilityManagement.Data
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDbContext _appDbContext;
        public RepositoryBase(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Create(T entity)
        {
            _appDbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _appDbContext.Set<T>().Remove(entity);
        }

        public IEnumerable<T> FindAll()
        {
            return _appDbContext.Set<T>().ToList();
        }

        public async Task<List<T>> FindAllAsync()
        {
            return await _appDbContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _appDbContext.Set<T>().Where(expression);
        }

        public T GetById(string id)
        {
            return _appDbContext.Set<T>().Find(id);
        }

        public T GetById(int id)
        {
            return _appDbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> GetWithOptions(QueryOptions<T> options)
        {
            IQueryable<T> query = _appDbContext.Set<T>();

            if (options.HasWhere)
                query = query.Where(options.Where);

            if (options.HasOrderBy)
            {
                if (options.OrderByDirection == "asc")
                    query = query.OrderBy(options.OrderBy);
                else
                    query = query.OrderByDescending(options.OrderBy);
            }
            if (options.HasPaging)
            {
                query = query.Skip((options.PageNumber - 1) * options.PageSize)
                             .Take(options.PageSize);
            }

            return query.ToList();
        }

        public void Update(T entity)
        {
            _appDbContext.Set<T>().Update(entity);
        }
    }
}
