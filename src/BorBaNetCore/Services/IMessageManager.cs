using BorBaNetCore.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BorBaNetCore.Services
{
	public interface IMessageManager
	{

		/// <summary>
		/// Create new message.
		/// </summary>
		/// <param name="message"></param>
		void Create(Messages message);

		/// <summary>
		/// Create new message from API.
		/// </summary>
		/// <param name="message"></param>
		Task ApiCreate(Messages message);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		Task Save(Messages message);

		void Delete(Messages message);


        void Update(Messages msg);
        Task<List<Messages>> List();
        Task<SearchResult<Messages>> Search(string name = null, int pageIndex = 0, int pageSize = Constants.DEFAULT_PAGE_SIZE, bool? isActive = null);

        Task<int> Save_sp(Messages message);
        //Task<SearchResult<Message_ListFiltered_Result>> Search(int? jobId, int? recipientId, string text, string senderName, int pageIndex = 0, int pageSize = CoreConstants.DEFAULT_PAGE_SIZE);

        //Task<IEnumerable<Message_ListFiltered_Result>> ApiSearch(int userId, DateTime? updatedFrom, DateTime? updatedTo, DateTime? dateFrom);

        //IEnumerable<Message_ListFiltered_Result> ListTopMessages(int count = CoreConstants.DEFAULT_PAGE_SIZE, bool isNotReadOnly = false);

        //Task<IEnumerable<Image>> ListImages(string imageIDs);
    }
}
