using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class GasMetingen
    {       
        public DateTime VerbruiksDatum { get; set; }
        public string DeviceID { get; set; }
        
        public GasDagMeting MetingLijst = new GasDagMeting();

    }
    public class GasDagMeting
    {
        public string meter_identifier { get; set; }

        public List<GasUsage> usages = new List<GasUsage>();

    }
    public class GasUsage
    {
        public string time { get; set; }
        public string delivery { get; set; }
        public string delivery_reading { get; set; }
        public string temperature { get; set; }
    }
}