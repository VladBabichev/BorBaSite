using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BorBaNetCore
{
	public class UnitOfWork
	{
		private IDictionary<Type, object> _repositories = new Dictionary<Type, object>();

		private DbContext _context;

		public UnitOfWork(DbContext context, Action<string> logger)
		{
			_context = context;			
		}

		public virtual DbSet<T> Set<T>() where T : class
		{
			return _context.Set<T>();
		}

		public virtual IRepository<TType, TId> GetRepository<TType, TId>() where TType : class
		{
			if (!_repositories.ContainsKey(typeof(TType)))
			{
				_repositories[typeof(TType)] = new GenericRepository<TType, TId>(_context);
			}
			return _repositories[typeof(TType)] as IRepository<TType, TId>;
		}

		public virtual IRepository<TType, int> GetRepository<TType>() where TType : class
		{
			return GetRepository<TType, int>();
		}

		public virtual void Save()
		{
			_context.SaveChanges();
		}
	}
}
