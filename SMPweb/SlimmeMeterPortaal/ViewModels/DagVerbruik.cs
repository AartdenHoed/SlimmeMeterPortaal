using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    // DagVerbruik bevat een lijst met 24 uurverbruiken, dit wordt verkregen door het consolideren van GasDagMeting resp. StroomDagMeting
    public class DagVerbruik 
    {
        public DateTime VerbruiksDatum { get; set; }
        public string MeterIdentificatie { get; set; }
        public string VerbruiksType { get; set; }
        
        public List<UurVerbruik> UurVerbruiken = new List<UurVerbruik>();
        public string UTC 
        { 
            get 
            { 
                int n = this.UurVerbruiken.Count;
                string ret = "UTC " + this.UurVerbruiken[0].UTC;
                // if UTC label changes during this day, display start + end UTC separated by slash 
                if (this.UurVerbruiken[0].UTC != this.UurVerbruiken[n-1].UTC)
                {
                    ret = ret + "/" + this.UurVerbruiken[n - 1].UTC;
                }
                return ret;
            }
        }

        public decimal TotaalperDag
        {
            get
            {
                decimal totaalperdag = 0;
                foreach (UurVerbruik entry in this.UurVerbruiken)
                {
                    totaalperdag += entry.Waarde;

                };
                return totaalperdag;
            }
        }
        public decimal DiffperDag
        {
            get
            {
                int cnt = this.UurVerbruiken.Count;
                // substract last endreading and first endreading. Than add the usage of the first hour
                decimal diff = this.UurVerbruiken[cnt - 1].EndReading - this.UurVerbruiken[0].EndReading + this.UurVerbruiken[0].Waarde;

                return diff;
            }
        }
    }
      
    public class UurVerbruik
    {
        public string UurLabel { get; set; }
        public string UTC { get; set; }
        public DateTime VerbruiksTijdstip { get; set; }
        public int UurNummer { get; set; }          
        public decimal Waarde { get; set; }
        public int AantalMetingen { get; set; } 
        public decimal EndReading { get; set; } // Meterstand op einde van het uur
    }    
    
}