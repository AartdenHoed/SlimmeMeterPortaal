using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class MaandVerbruik
    {
        public int Jaar { get; set; }
        public string DeviceID { get; set; }
        public string VerbruiksType { get; set; }
        
        public List<MaandCijfer> MaandCijfers = new List<MaandCijfer>();

        public decimal TotaalperJaar
        {
            get
            {
                decimal totaalperjaar = 0;
                foreach (MaandCijfer m in this.MaandCijfers)
                {
                    totaalperjaar += m.Cijfer;

                };
                return totaalperjaar;
            }
        }
    }
      
    public class MaandCijfer
    {
        public string MaandLabel {  get; set; }
        public int MaandNummer { get; set; }          
        public decimal Cijfer { get; set; }
        public decimal MeterstandTotaal { get; set; }
        public decimal MeterstandNormaalTarief { get; set; }
        public decimal MeterstandLaagTarief { get; set; }
       
    }    
    
}