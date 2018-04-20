using System;
using System.Linq;
using BorBaNetCore.Extensions;
namespace BorBaNetCore.DataModel
{
	public partial class Users
	{
		public void SetPassword(string password)
		{
			if (Salt.IsEmpty())
			{
				Salt = Guid.NewGuid().ToString();
			}
			PasswordHash = (password + Salt).MD5Hash();
		}

		public bool IsAdmin
		{
			get { return UserRoles != null && UserRoles.Any(ur => ur.Role.Abbrev.Contains("Admin", ignoreCase: true)); }
		}

		public bool IsAbleToDelete()
		{
            return true;
		}

    }
}
