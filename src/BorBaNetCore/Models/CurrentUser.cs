using BorBaNetCore.Extensions;
using System;
using System.Linq;


namespace BorBaNetCore.Models
{
	/// <summary>
	/// User in current context.
	/// </summary>
	[Serializable]
	public class CurrentUser
	{
		/// <summary>
		/// User name
		/// </summary>
		public string UserName { get; set; }
        public string EMail { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName
		{
			get
			{
				if (LastName.IsEmpty())
				{
					if (FirstName.IsEmpty())
					{
						return UserName;
					}
					return FirstName;
				}
				else
				{
					if (FirstName.IsEmpty())
					{
						return LastName;
					}
					return FirstName + " " + LastName;
				}
			}
		}

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Last name
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// User id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Company id
		/// </summary>
		public int? CompanyId { get; set; }


		/// <summary>
		/// Indicates if the admin has profile image uploaded
		/// </summary>
		public bool HasImage { get; set; }
        public bool IsAdmin = false;
    }
}