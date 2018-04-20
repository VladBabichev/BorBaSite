using System;
using System.Collections.Generic;

namespace BorBaNetCore.DataModel
{
    public partial class Images
    {
        public int ImageId { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
