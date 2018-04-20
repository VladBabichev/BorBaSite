using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BorBaNetCore.Services.Impl
{
	public abstract class BaseManager<T> where T : class
	{
		protected UnitOfWork unitOfWork { get; private set; }

		protected BaseManager(UnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		protected IRepository<T, int> items
		{
			get
			{
				return unitOfWork.GetRepository<T>();
			}
		}

	}
}
