using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class Stats
    {
        public List<UurStats> UurStatsList = new List<UurStats>();
        public DagStats DagStats = new DagStats();
        public int GetLevel(decimal d, StatNumbers statnumbers)
        {

            if (d < statnumbers.Perc000)
            {
                return -3;
            }
            if (d < statnumbers.Perc020)
            {
                return -2;
            }
            if (d < statnumbers.Perc040)
            {
                return -1;
            }
            if (d <= statnumbers.Perc060)
            {
                return 0;
            }
            if (d <= statnumbers.Perc080)
            {
                return 1;
            }
            if (d <= statnumbers.Perc100)
            {
                return 2;
            }
            return 3;

        }
    }

    public class UurStats
    {
        public List<decimal> Uurwaarden = new List<decimal>();
        public int AantalWaarnemingen { get; set; }
        public string UurLabel { get; set; }
        
        public StatNumbers StatNumbers = new StatNumbers();

    }

    public class DagStats
    {
        public List<decimal> Dagwaarden = new List<decimal>();
        public int AantalWaarnemingen { get { return Dagwaarden.Count; } }

        public List<DateTime> Datums = new List<DateTime>();

        public StatNumbers StatNumbers = new StatNumbers();
    }

    public class StatNumbers
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