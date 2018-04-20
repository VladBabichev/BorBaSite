using System;
using System.Collections.Generic;
using BorBaNetCore.DataModel;

namespace BorBaNetCore.Services
{

	public class SearchResult<T>
	{
		public int TotalCount { get; set; }

		/// <summary>
		/// Page index. 0 based.
		/// </summary>
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public IList<T> Items { get; set; }

		public int TotalPages
		{
			get
			{
				return TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
			}
		}

		public SearchResult<U> Transform<U>(Func<T, U> converter)
		{
			var result = new SearchResult<U>() { PageIndex = PageIndex, PageSize = PageSize, TotalCount = TotalCount, Items = new List<U>() };

			foreach (T item in Items)
			{
				result.Items.Add(converter(item));
			}

			return result;
		}

        public static implicit operator SearchResult<T>(List<Messages> v)
        {
            throw new NotImplementedException();
        }
    }

}
