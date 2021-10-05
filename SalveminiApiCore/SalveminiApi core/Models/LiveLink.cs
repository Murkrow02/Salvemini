using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class LiveLink
    {
        public string Link { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Title { get; set; }
    }
}
