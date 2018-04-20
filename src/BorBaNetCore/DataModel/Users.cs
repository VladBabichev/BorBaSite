using System;
using System.Collections.Generic;

namespace BorBaNetCore.DataModel
{
    public partial class Users
    {
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
            UserTokens = new HashSet<UserTokens>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSystem { get; set; }

        public virtual ICollection<UserRoles> UserRoles { get; set; }
        public virtual ICollection<UserTokens> UserTokens { get; set; }
    }
}
