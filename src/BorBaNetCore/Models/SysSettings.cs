using System;
using System.Collections.Generic;

namespace BorBaNetCore.Models
{
    public partial class SysSettings
    {
        public int CompanyId { get; set; }
        public int MaxSchedulingPeriod { get; set; }
        public int MinJobInterval { get; set; }
        public int? DashboardTopJobs { get; set; }
    }
}
