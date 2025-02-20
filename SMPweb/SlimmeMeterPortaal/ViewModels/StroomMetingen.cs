using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class StroomMetingen
    {
        public DateTime VerbruiksDatum { get; set; }
        public string DeviceID { get; set; }
        
        public StroomDagMeting MetingLijst = new StroomDagMeting();
    }

    public class StroomDagMeting
    {        
        public string meter_identifier { get; set; }

        public List<StroomUsage> usages = new List<StroomUsage>();

    }
    public class StroomUsage
    {
        public string time { get; set; }
        public string delivery_high { get; set; }
        public string delivery_low { get; set; }
        public string delivery_reading_high { get; set; }
        public string delivery_reading_low { get; set; }
        public string delivery_reading_combined { get; set; }
        public string returned_delivery_high { get; set; }
        public string returned_delivery_low { get; set; }
        public string returned_delivery_reading_high { get; set; }
        public string returned_delivery_reading_low { get; set; }
        public string returned_delivery_reading_combined { get; set; }
        public string temperature { get; set; }
    }
}