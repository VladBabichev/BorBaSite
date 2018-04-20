using System;
using System.Collections.Generic;

namespace BorBaNetCore.DataModel
{
    public partial class UserTokens
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }

        public virtual Users User { get; set; }
    }
}
