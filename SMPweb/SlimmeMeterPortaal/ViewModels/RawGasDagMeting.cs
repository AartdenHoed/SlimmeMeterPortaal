using System;
using System.Collections.Generic;

namespace SlimmeMeterPortaal.ViewModels
{
    public class RawGasDagMeting
    {       
        public DateTime VerbruiksDatum { get; set; }
        public string MeterIdentificatie { get; set; }
        
        public GasTimeSlotMetingLijst GasTimeSlotMetingLijst = new GasTimeSlotMetingLijst();

    }
    // Hieronder de ruwe datastructuur zoals die door de API wordt aangeleverd, per datum. 
    public class GasTimeSlotMetingLijst
    {
        public string meter_identifier { get; set; }

        public List<GasTimeSlotMeting> usages = new List<GasTimeSlotMeting>();

    }
    public class GasTimeSlotMeting
    {
        public string time { get; set; }
        public string delivery { get; set; }
        public string delivery_reading { get; set; }
        public string temperature { get; set; }
    }
}