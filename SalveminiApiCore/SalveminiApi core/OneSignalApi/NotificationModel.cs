using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core.OneSignalApi
{
    public class NotificationModel
    {
        public string app_id = "a85553ca-c1fe-4d93-a02f-d30bf30e2a2a";
        public string[] included_segments = { "All" };
        public Localized contents { get; set; }
        public Localized headings { get; set; }
        public List<Tags> filters { get; set; }
        public AdditionalData data { get; set; }
    }

    public class Tags
    {
        public string field { get; set; }
        public string key { get; set; }
        public string relation { get; set; }
        public string value { get; set; }
    }

    public class Localized
    {
        public string en { get; set; }
    }

    public class AdditionalData
    {
        public string tipo { get; set; }
        public string id { get; set; }
    }
}