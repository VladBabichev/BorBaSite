using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BorBaNetCore.Classes;
using BorBaNetCore.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace BorBaNetCore.Services.Impl
{
	public class MessageManager : BaseManager<Messages>, IMessageManager
	{
      
        private DbContext _dataContext { get; set; }
        private UnitOfWork _unitOfWork { get; set; }

        public MessageManager(UnitOfWork unitOfWork, DbContext dataContext) : base(unitOfWork)
		{
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;

        }

        #region ' private properties
        //private IRepository<Contact, int> contacts
        //{
        //	get
        //	{
        //		return unitOfWork.GetRepository<Contact>();
        //	}
        //}
        #endregion

        public void Update(Messages msg)
        {
           // procs.Message_Update(msg.MessageId, msg.Name, msg.Subject, msg.Email, msg.Text);
        }
        public void Create(Messages message)
		{
			//CurrentUser currentAdmin = _userManager.CurrentUser;
			items.Insert(message);
			unitOfWork.Save();
		}

        public void ApiCreate(Messages message)
        {
            items.Insert(message);

            unitOfWork.Save();
            //         var user = await _userManager.GetSystemUser();
            //if (user != null)
            //{
            //	//message.Outgoing = false;
            //	//message.MessageRecipients.Add(new MessageRecipient
            //	//{
            //	//	Message = message,
            //	//	RecipientId = user.UserId,
            //	//	SyncedOn = DateTime.Now,
            //	//	UpdatedOn = DateTime.Now
            //	//});

            //}
        }

        public async Task Save(Messages message)
		{
            _dataContext.Add(message);
            await _dataContext.SaveChangesAsync();

            //await _dataContext.Database.ExecuteSqlCommandAsync("Message_Update @Name, @Subject,@Email,@Text" , parameters: new[] { message.Name, message.Subject, message.Email, message.Text });
            //CurrentUser currentAdmin = _userManager.CurrentUser;
            //items.Update(message);
            //unitOfWork.Save();

        }
        public async Task<int> Save_sp(Messages message)
        {
            DbCommand cmd = _dataContext.Database.GetDbConnection().CreateCommand();

            cmd.CommandText = "dbo.Message_Update";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = message.Name });
            cmd.Parameters.Add(new SqlParameter("@Subject", SqlDbType.NVarChar) { Value = message.Subject });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = message.Email });
            cmd.Parameters.Add(new SqlParameter("@Text", SqlDbType.NText) { Value = message.Text });

            cmd.Parameters.Add(new SqlParameter("@MessageId", SqlDbType.Int) { Direction = ParameterDirection.Output });

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            await cmd.ExecuteNonQueryAsync();

            int id = (int)cmd.Parameters["@MessageId"].Value;
            return id;
        }
        public void Delete(Messages message)
        {
            //var procResult = procs.Message_Delete(message.MessageId);
            items.Delete(message);
            unitOfWork.Save();
            //return procResult == -1;
        }

        Task IMessageManager.ApiCreate(Messages message)
        {
            throw new NotImplementedException();
        }

        public async Task<SearchResult<Messages>> Search(string name = null, int pageIndex = 0, int pageSize = Constants.DEFAULT_PAGE_SIZE, bool? isActive = null)
        {
            var msg = _unitOfWork.GetRepository<Messages>();

            return await unitOfWork.GetRepository<Messages>().SearchAndCount(
                set =>
                {
                    var result = set;                   
                  
                    return result;
                },
                c => c.MessageId,
                pageSize: pageSize,
                pageIndex: pageIndex
            );
        }
        public async Task<List<Messages>> List()
        {
            List<Messages> list =await _dataContext.Set<Messages>().ToListAsync();
            return list; 
        }

        //public async Task<SearchResult<Message_ListFiltered_Result>> Search(int? jobId, int? recipientId, string text, string senderName, int pageIndex = 0, int pageSize = CoreConstants.DEFAULT_PAGE_SIZE)
        //{
        //    bool? isSentToAdmin = null;
        //    var list = await procs.Message_ListFiltered(jobId, recipientId, isSentToAdmin, senderName, text, null, null, null).ToListAsync();
        //    return new SearchResult<Message_ListFiltered_Result>()
        //    {
        //        PageSize = pageSize,
        //        PageIndex = pageIndex,
        //        TotalCount = list.Count,
        //        Items = list.Skip(pageIndex * pageSize).Take(pageSize).ToList()
        //    };
        //}

        //public async Task<IEnumerable<Message_ListFiltered_Result>> ApiSearch(int userId, DateTime? updatedFrom, DateTime? updatedTo, DateTime? dateFrom)
        //{
        //	return await procs.Message_ListFiltered(
        //					null, userId, false, null, null,
        //					updatedFrom, updatedTo, dateFrom
        //				).ToListAsync();
        //}

        //public IEnumerable<Message_ListFiltered_Result> ListTopMessages(int count = CoreConstants.DEFAULT_PAGE_SIZE, bool isNotReadOnly = false)
        //{
        //	bool isSentToAdmin = true;
        //	return procs.Message_ListFiltered(null, null, isSentToAdmin, null, null, null, null, null)
        //						.Where(a => !isNotReadOnly || !(a.IsRead ?? false))
        //						.Take(count);
        //}

        //public async Task<IEnumerable<Image>> ListImages(string imageIDs)
        //{
        //	if (imageIDs.IsEmpty())
        //		return new Image[0];

        //	int[] ids = imageIDs.ToEnumInt32().ToArray();
        //	return await unitOfWork.GetRepository<Image>().Search(img => ids.Contains(img.ImageId), img => img.ImageId.ToString());
        //}
    }
}
