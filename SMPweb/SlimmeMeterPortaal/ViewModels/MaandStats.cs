using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class MaandStats
    {
        public List<MaandGegeven> MaandGegevens = new List<MaandGegeven>();

        public JaarStats JaarStats = new JaarStats();

        public int GetLevel(decimal d, MYStatistiek m)
        {

            if (d < m.Min)
            {
                return -3;
            }
            if (d < m.Laag)
            {
                return -2;
            }
            if (d < m.Verlaagd)
            {
                return -1;
            }
            if (d <= m.Verhoogd)
            {
                return 0;
            }
            if (d <= m.Hoog)
            {
                return 1;
            }
            if (d <= m.Max)
            {
                return 2;
            }
            return 3;

        }
    }
        
    public class MaandGegeven {
        public int Maandnr { get; set; }
        public string MaandLabel { get; set; }
        public int AantalWaarnemingen { get; set; }

        public MYStatistiek MaandStatistiek = new MYStatistiek();
    }
        
    public class JaarStats
    {
        public int AantalWaarnemingen { get { return Jaren.Count; } }

        public List<int> Jaren = new List<int>();

        public MYStatistiek MYStatistiek = new MYStatistiek();
    }

    public class MYStatistiek
    {
        public decimal Laag { get
            {
                decimal d = this.Mean - ((this.Mean - this.Min) * 2 / 3);
                return d;
            }
       }
        public decimal Verlaagd
        {
            get
            {
                decimal d = this.Mean - ((this.Mean - this.Min) * 1 / 3);
                return d;
            }
        }
        public decimal Verhoogd
        {
            get
            {
                decimal d = this.Mean + ((this.Max - this.Mean) * 1 / 3);
                return d;
            }
        }
        public decimal Hoog
        {
            get
            {
                decimal d = this.Mean + ((this.Max - this.Mean) * 2 / 3);
                return d;
            }
        }
        
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Mean { get; set; }        
    }
}