using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class DagVerbruik
    {
        public DateTime VerbruiksDatum { get; set; }
        public string DeviceID { get; set; }
        public string VerbruiksType { get; set; }
        
        public List<UurVerbruikEntry> UurLijst = new List<UurVerbruikEntry>();

        public decimal TotaalperDag
        {
            get
            {
                decimal totaalperdag = 0;
                foreach (UurVerbruikEntry entry in this.UurLijst)
                {
                    totaalperdag += entry.UurVerbruik;

                };
                return totaalperdag;
            }
        }
    }
      
    public class UurVerbruikEntry
    {
        public string UurLabel {  get; set; }
        public DateTime VerbruiksTijdstip { get; set; }
        public int UurNummer { get; set; }          
        public decimal UurVerbruik { get; set; }
        public int AantalMetingen { get; set; } 

    }    
    
}