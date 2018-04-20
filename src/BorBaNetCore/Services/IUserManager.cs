
using BorBaNetCore.Models;
using System.Threading.Tasks;
using BorBaNetCore.DataModel;
namespace BorBaNetCore.Services
{
    public partial interface IUserManager
    {
        /// <summary>
        /// Get or set the current use for the context
        /// </summary>
        CurrentUser CurrentUser { get; set; }

        /// <summary>
        /// Login user with the given userName and password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Users> Login(string userName, string password);

        /// <summary>
        /// Create user account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password">Clear text password for the user account</param>
        /// <returns></returns>
        Task Create(Users user, string password);
        Task<Users> Create(string userName, string password);

        /// <summary>
        /// Get admin by the given id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Users> Get(int userId);

        Task<Users> GetSystemUser();

        /// <summary>
        /// Get admin by the given userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Users> GetByName(string userName);

        /// <summary>
        /// Get admin by remember me token
        /// </summary>
        /// <param name="rememberMeToken"></param>
        /// <returns></returns>
        Task<Users> GetByToken(string rememberMeToken);

        /// <summary>
        /// Get remember token for this admin user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetToken(int userId);

        Task<CurrentUser> ApiGetByToken(string authToken);

        Task<string> ApiGetToken(string userName, string password);

        Task ApiDiscardToken(CurrentUser user, string token);

    }
}
