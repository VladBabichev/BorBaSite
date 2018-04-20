using BorBaNetCore.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using BorBaNetCore.Extensions;
using BorBaNetCore.DataModel;
namespace BorBaNetCore.Models
{
	public static class Extensions
	{
		#region IAdminManager

		public static void SetCurrentUser(this IUserManager adminManager, HttpContext httpContext, CurrentUser admin)
		{
			adminManager.CurrentUser = admin;
			if (admin == null)
			{
				httpContext.Session.Remove(Constants.Session.CURRENT_USER);
				return;
			}
			else
			{
				httpContext.Session.SetInt32(Constants.Session.CURRENT_USER, admin.Id);
			}
		}

		public static async Task<CurrentUser> GetCurrentUser(this IUserManager adminManager, HttpContext httpContext)
		{
          
            if (adminManager.CurrentUser != null)
			{
				return adminManager.CurrentUser;
			}
			Users admin;
			int? adminId = httpContext.Session.GetInt32(Constants.Session.CURRENT_USER);
			if (adminId.HasValue)
			{
				admin = await adminManager.Get(adminId.Value);
				CurrentUser currentUser = admin == null ? null : admin.ToCurrentUser();
				SetCurrentUser(adminManager, httpContext, currentUser);
				return currentUser;
			}

			// User not logged in. Look for remember me token in request cookie
			string remeberMeToken = httpContext.Request.Cookies[Constants.Cookie.REMEMBER_ME];

			// if no remember me token, return null
			if (remeberMeToken == null)
			{
				return null;
			}

			// Get user by remember me token
			admin = await adminManager.GetByToken(remeberMeToken);
			if (admin != null)
			{
				// Convert the user to current user and store in session
				CurrentUser currentUser = admin.ToCurrentUser();
				SetCurrentUser(adminManager, httpContext, currentUser);
				return currentUser;
			}

			// If user is still not found, return null
			return null;
		}

		public static async Task<bool> IsCurrentAdmin(this IUserManager adminManager, HttpContext httpContext)
		{
			var currUser = await adminManager.GetCurrentUser(httpContext);
			return await IsCurrentAdmin(adminManager, currUser);
		}

		public static async Task<bool> IsCurrentAdmin(this IUserManager adminManager, CurrentUser currUser)
		{
			var foundUser = await adminManager.Get(currUser.Id);
			//return foundUser.IsAdmin;
            return false;
		}

		#endregion


		/// <summary>
		/// Returns the value if it falls within the min/max boundaries or the boundary value - otherwise
		/// </summary>
		public static double Within(this double value, double min, double max)
		{
			return Math.Min(max, Math.Max(min, value));
		}

		public static bool IsEmpty(this PathString value)
		{
			return value == null || value.Value.IsEmpty();
		}
		public static bool IsEmpty(this QueryString value)
		{
			return value == null || value.Value.IsEmpty();
		}

		public static bool IsNotEmpty(this PathString value)
		{
			return value != null && value.Value.IsNotEmpty();
		}
		public static bool IsNotEmpty(this QueryString value)
		{
			return value != null && value.Value.IsNotEmpty();
		}

		public static string FirstValue(this IFormCollection collection, string name)
		{
			return collection != null && collection[name].Count != 0 ? collection[name][0] : null;
		}

		public static bool CancelClicked(this HttpRequest request)
		{
			return request != null && request.Form.FirstValue("submit").IsSameAs("Cancel", true);
		}

		public static bool SaveClicked(this HttpRequest request)
		{
			return request != null && request.Form.FirstValue("submit").IsSameAs("Save", true);
		}
		
	}
}
