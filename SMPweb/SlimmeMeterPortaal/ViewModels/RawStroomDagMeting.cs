using System;
using System.Collections.Generic;

namespace SlimmeMeterPortaal.ViewModels
{
    public class RawStroomDagMeting
    {
        public DateTime VerbruiksDatum { get; set; }
        public string MeterIdentificatie { get; set; }
        
        public StroomTimeSlotMetingLijst StroomTimeSlotMetingLijst = new StroomTimeSlotMetingLijst();
    }

    // Hieronder de ruwe datastructuur zoals die door de API wordt aangeleverd, per datum. 
    public class StroomTimeSlotMetingLijst
    {        
        public string meter_identifier { get; set; }

        public List<StroomTimeSlotMeting> usages = new List<StroomTimeSlotMeting>();

    }
    public class StroomTimeSlotMeting
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