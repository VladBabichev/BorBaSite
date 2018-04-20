using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BorBaNetCore
{
	public interface IDataContext : IDisposable
	{	

		DbSet<TEntity> Set<TEntity>() where TEntity : class;

		int SaveChanges();

		Task<int> SaveChangesAsync();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
