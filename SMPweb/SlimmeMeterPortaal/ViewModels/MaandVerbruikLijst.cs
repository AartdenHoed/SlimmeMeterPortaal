using System.Collections.Generic;

namespace SlimmeMeterPortaal.ViewModels
{
    public class MaandVerbruikLijst
    {
        public int Jaar { get; set; }
        public string MeterIdentificatie { get; set; }
        public string VerbruiksType { get; set; }
        
        public List<MaandVerbruik> MaandVerbruiken = new List<MaandVerbruik>();

        public decimal TotaalperJaar
        {
            get
            {
                decimal totaalperjaar = 0;
                foreach (MaandVerbruik m in this.MaandVerbruiken)
                {
                    totaalperjaar += m.Waarde;

                };
                return totaalperjaar;
            }
        }
    }
      
    public class MaandVerbruik
    {
        public string MaandLabel {  get; set; }
        public int MaandNummer { get; set; }          
        public decimal Waarde { get; set; }
        public decimal MeterstandTotaal { get; set; }
        public decimal MeterstandNormaalTarief { get; set; }
        public decimal MeterstandLaagTarief { get; set; }
       
    }    
    
}