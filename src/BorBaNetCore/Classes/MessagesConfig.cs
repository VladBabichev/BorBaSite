using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BorBaNetCore.Classes
{
    public class MessagesConfig
    {
        public bool SmtpEnabled { get; set; }
        public bool DataBaseEnabled { get; set; }
        public bool RemoteDataBaseEnabled { get; set; }
        public string RemoteDataBaseURL { get; set; }
    }
}
