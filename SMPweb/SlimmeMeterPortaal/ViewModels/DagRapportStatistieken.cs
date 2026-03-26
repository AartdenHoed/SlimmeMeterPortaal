using System;
using System.Collections.Generic;

namespace SlimmeMeterPortaal.ViewModels
{
    public class DagRapportStatistieken
    {
        public List<UurVerbruiksWaarden> UurVerbruiksWaardenLijst = new List<UurVerbruiksWaarden>();
        public DagVerbruiksWaarden DagVerbruiksWaarden = new DagVerbruiksWaarden();
        public int GetLevel(decimal d, StatistiekWaarden StatistiekWaarden)
        {

            if (d < StatistiekWaarden.Perc000)
            {
                return -3;
            }
            if (d < StatistiekWaarden.Perc020)
            {
                return -2;
            }
            if (d < StatistiekWaarden.Perc040)
            {
                return -1;
            }
            if (d <= StatistiekWaarden.Perc060)
            {
                return 0;
            }
            if (d <= StatistiekWaarden.Perc080)
            {
                return 1;
            }
            if (d <= StatistiekWaarden.Perc100)
            {
                return 2;
            }
            return 3;

        }
    }

    public class UurVerbruiksWaarden
    {
        // Bevat voor een specifiek uur alle referentie waarden en bijbehorende statistieken (percentielen, N, Min, Max) 
        public List<decimal> Uurwaarden = new List<decimal>();
        public int AantalWaarnemingen { get; set; }
        public string UurLabel { get; set; }
        
        public StatistiekWaarden StatistiekWaarden = new StatistiekWaarden();

    }

    public class DagVerbruiksWaarden
    {
        // Bevat voor 1 dag alle referentie waarden en bijbehorende statistieken (percentielen, N, Min, Max). 
        // NB: Elke dag heeft andere referenite waarden (namelijk per volgende dag een dag verschoven naar de toekomst)
        public List<decimal> Dagwaarden = new List<decimal>();
        public int AantalWaarnemingen { get { return Dagwaarden.Count; } }

        public List<DateTime> Datums = new List<DateTime>();

        public StatistiekWaarden StatistiekWaarden = new StatistiekWaarden();
    }

    public class StatistiekWaarden
    {
        public decimal Perc000 { get; set; }
        public decimal Perc020 { get; set; }
        public decimal Perc040 { get; set; }
        public decimal Perc060 { get; set; }
        public decimal Perc080 { get; set; }
        public decimal Perc100 { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Mean { get; set; }        
    }
}