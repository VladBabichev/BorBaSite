
using BorBaNetCore.Services;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BorBaNetCore
{
   
    public interface IRepository<TEntity, TId> where TEntity : class
	{

		Task<IEnumerable<TEntity>> Search<TSort>(Expression<Func<TEntity, bool>> filter = null,
			Expression<Func<TEntity, TSort>> orderBy = null,
			string includeProperties = "");

		Task<IEnumerable<TEntity>> Search<TSort>(Func<IQueryable<TEntity>, IQueryable<TEntity>> filterFunc,
			Expression<Func<TEntity, TSort>> orderBy,
			string includeProperties = "", int pageSize = Constants.DEFAULT_PAGE_SIZE, int pageIndex = 0);

		Task<SearchResult<TEntity>> SearchAndCount<TSort>(Func<IQueryable<TEntity>, IQueryable<TEntity>> filterFunc,
			Expression<Func<TEntity, TSort>> orderBy,
			string includeProperties = "", int pageSize = Constants.DEFAULT_PAGE_SIZE, int pageIndex = 0);
		Task<IEnumerable<TEntity>> Search<TSort>(Func<IQueryable<TEntity>, IQueryable<TEntity>> filterFunc,
			IEnumerable<Expression<Func<TEntity, TSort>>> orderBy,
			string includeProperties = "", int pageSize = Constants.DEFAULT_PAGE_SIZE, int pageIndex = 0);

		Task<SearchResult<TEntity>> SearchAndCount<TSort>(Func<IQueryable<TEntity>, IQueryable<TEntity>> filterFunc,
			IEnumerable<Expression<Func<TEntity, TSort>>> orderBy,
			string includeProperties = "", int pageSize = Constants.DEFAULT_PAGE_SIZE, int pageIndex = 0);
		TEntity Get(Expression<Func<TEntity, bool>> predicate, string include = null);
		TEntity GetByID(TId id);

		TEntity GetBy(Expression<Func<TEntity, bool>> predicate);

		void Insert(TEntity entity);

		void Delete(TId id);

		void Delete(TEntity entityToDelete);

		void Update(TEntity entityToUpdate);
	}
}
