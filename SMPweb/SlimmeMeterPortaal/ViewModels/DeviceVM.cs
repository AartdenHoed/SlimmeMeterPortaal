using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class DeviceVM
    {
        [DisplayName("Meenemen in rapportage")]
        public bool ReportDevice { get; set; } = true;

        [DisplayName("Meter identificatie")]
        public string DeviceID { get; set; }

        [DisplayName("Meter type")]
        public string DeviceType { get; set; }

        [DisplayName("Meter start datum")]
        public System.DateTime Startdate { get; set; }

        [DisplayName("Meter eind datum")]
        public Nullable<System.DateTime> Enddate { get; set; }
    }
}