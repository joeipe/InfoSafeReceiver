﻿using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfoSafeReceiver.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        void Create(params TEntity[] items);

        void Update(params TEntity[] items);

        void Delete(params TEntity[] items);

        Task SaveAsync();

        Task<TEntity> FindAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllIncludeAsync(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties);

        Task<IEnumerable<TEntity>> SearchForAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> SearchForIncludeAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties);
    }
}
