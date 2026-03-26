using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    // Dit is de ruwe datastructuur zoals de lijst met verbruiksmeters wordt aangeleverd via de API
    public class RawMeter
    {
    public string meter_identifier { get; set; }
    public string connection_type { get; set; }
    public string start_date { get; set; }
    public string end_date {  get; set; }    
    }
}