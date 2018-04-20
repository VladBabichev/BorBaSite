
using System;
using System.Linq;
using System.Threading.Tasks;
using BorBaNetCore.Models;
using BorBaNetCore.DataModel;
using Microsoft.EntityFrameworkCore;
using BorBaNetCore.Extensions;
using BorBaNetCore.Services;

namespace BorBaNetCore.Services.Impl
{
	/// <summary>
	/// This manager is to handle users authentications for API access. 
	/// </summary>
	public partial class UserManager : IUserManager
	{
		private const string _REMEMBER_ME_TOKEN = "RememberMeToken";

		private DbSet<Users> _users;
		private DbSet<UserTokens> _userTokens;
		private DbContext _dataContext { get; set; }

		public UserManager(DbContext dbContext)
		{
			_dataContext = dbContext;
		}

		#region Properties
		public virtual CurrentUser CurrentUser { get; set; }

		protected virtual DbSet<Users> users
		{
			get
			{
				return _users ?? (_users = _dataContext.Set<Users>());
			}
		}

		protected virtual DbSet<UserTokens> userTokens
		{
			get
			{
				return _userTokens ?? (_userTokens = _dataContext.Set<UserTokens>());
			}
		}

      

        #endregion

        #region ' private methods
        async private Task<Users> getUser(string userName, Func<Users, string> getPasswordHash)
		{
			Users result = await GetByName(userName);
			string hash;
			if (result == null
			  || getPasswordHash == null
			  || (hash = getPasswordHash(result)).IsEmpty()
			  || result.PasswordHash.IsNotSameAs(hash, ignoreCase: true))
				result = null;

			return result;
		}

		private static bool getUserName(string remeberMeToken, out string userName, out string hash)
		{
			bool result = false;
			userName = hash = null;
			try
			{
				var tokens = remeberMeToken.Decrypt(_REMEMBER_ME_TOKEN).Split(';');
				if (tokens.Length == 3)
				{
					userName = tokens[0];
					hash = tokens[1];
					result = true;
				}
			}
			catch { }
			return result;
		}
		#endregion

		/// <summary>
		/// Create user account
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		async public Task Create(Users user, string password)
		{
			Users existingUser = await GetByName(user.UserName);
			if (existingUser != null)
			{
				throw new Exception("User name already exists");
			}

			user.SetPassword(password);
			users.Add(user);
			await _dataContext.SaveChangesAsync();
			return;
		}

        async public Task<Users> Create(string userName, string password)
        {
            Users existingUser = await GetByName(userName);
            if (existingUser == null)
            {
                existingUser = new Users() {IsActive=true,UserName= userName };
                existingUser.SetPassword(password);
                users.Add(existingUser);
                await _dataContext.SaveChangesAsync();
            }           
         
            return existingUser;
        }

        async public Task<Users> CreateAdmin()
        {
            Users existingUser = await GetByName("admin");
            if (existingUser == null)
            {
                existingUser = new Users() { IsActive = true, UserName = "admin" ,IsSystem=true};
                existingUser.SetPassword("admin");
                users.Add(existingUser);
                await _dataContext.SaveChangesAsync();
            }

            return existingUser;
        }

        async public Task<Users> Get(int userId)
        {
            return await users.FirstAsync(u => u.UserId == userId);
        }

        async public Task<Users> GetByName(string userName)
		{
			return await users.FirstOrDefaultAsync(u => string.Compare(u.UserName, userName, true) == 0);
		}

		async public Task<Users> Login(string userName, string password)
		{
			return await getUser(userName, user => (password + user.Salt).MD5Hash());
		}

		#region Remember me token handling

		public async Task<Users> GetByToken(string rememberMeToken)
		{
			Users result;
			string userName, passwordHash;
			if (rememberMeToken.IsNotEmpty() && getUserName(rememberMeToken, out userName, out passwordHash))
			{
				result = await getUser(userName, a => passwordHash);
			}
			else
				result = null;

			return result;
		}

		public async Task<string> GetToken(int userId)
		{
			Users user = await Get(userId);

			if (user != null && user.UserId > 0)
				return string.Join(";", user.UserName, user.PasswordHash, DateTime.Now).Encrypt(_REMEMBER_ME_TOKEN);

			return string.Empty;
		}

		#endregion

		#region API Token Management

		private const int DEFAULT_SLIDING_EXPIRY_DURATION_IN_HOURS = 4;

		public async Task<CurrentUser> ApiGetByToken(string authToken)
		{
			DateTime now = DateTime.Now;
			UserTokens token = await userTokens.FirstOrDefaultAsync(t => t.Token == authToken);
			if (token != null && token.ExpiryTime > now)
			{
				// Sliding expiration
				token.ExpiryTime = now.AddHours(DEFAULT_SLIDING_EXPIRY_DURATION_IN_HOURS);
				return token.User.ToCurrentUser();
			}
			return null;
		}

		public async Task<string> ApiGetToken(string userName, string password)
		{
			Users user = await Login(userName, password);
			if (user == null || !(bool)user.IsActive)
				return null;

			//TODO: change the token generation logic so its more secure
			UserTokens token = new UserTokens()
			{
				ExpiryTime = DateTime.Now.AddHours(DEFAULT_SLIDING_EXPIRY_DURATION_IN_HOURS),
				User = user,
				Token = Guid.NewGuid().ToString()
			};
			user.UserTokens.Add(token);
			await _dataContext.SaveChangesAsync();
			return token.Token;
		}

		public async Task ApiDiscardToken(CurrentUser user, string token)
		{
			UserTokens ut = await userTokens.FirstOrDefaultAsync(t => t.Token == token && t.UserId == user.Id);
			if (ut != null)
			{
				userTokens.Remove(ut);
				await _dataContext.SaveChangesAsync();
			}
		}

        public async Task<Users> GetSystemUser()
        {
            return await users.FirstOrDefaultAsync(u => (bool)u.IsSystem);
        }

        #endregion
    }
}