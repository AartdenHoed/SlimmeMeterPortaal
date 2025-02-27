using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class DeviceVM 
    {
        public DeviceVM(string devtype, string gas, string stroom)
        {
            this.DeviceType = devtype;
            if (devtype == "elektriciteit")
            {
                if (stroom == "Y")
                { 
                    this.ReportDevice = true; 
                }
                else
                {
                    this.ReportDevice = false;
                }
            }
            if (devtype == "gas")
            {
                if (gas == "Y")
                {
                    this.ReportDevice = true;
                }
                else
                {
                    this.ReportDevice = false;
                }
            }
        }
        [DisplayName("Meenemen in rapportage")]
        public bool ReportDevice { get; set; } 

        [DisplayName("Meter identificatie")]
        public string DeviceID { get; set; }

        [DisplayName("Meter type")]
        public string DeviceType { get; set; }

        [DisplayName("Meter start datum")]
        public System.DateTime Startdate { get; set; }

        [DisplayName("Meter eind datum")]
        public Nullable<System.DateTime> Enddate { get; set; }

        // Lijsten met ruwe meter data
        public List<GasMetingen> ReferenceGasMetingen = new List<GasMetingen>();
        public List<StroomMetingen> ReferenceStroomMetingen = new List<StroomMetingen>();
        public List<GasMetingen> UsageGasMetingen = new List<GasMetingen>();
        public List<StroomMetingen> UsageStroomMetingen = new List<StroomMetingen>();

        // Lijsten met per uur geconsolideerde data 
        public List<DagVerbruik> UurVerbruik = new List<DagVerbruik>();
        public List<DagVerbruik> UurReference = new List<DagVerbruik>();
        
        // Lijsten met statistieken 
        public Stats Statistieken = new Stats();

        public List<RPT_line> DagRapport = new List<RPT_line>();
        public class RPT_line
        {
            public string N { get; set; }
            public string Min { get; set; }
            public string Mean { get; set; }
            public string Max { get; set; }
            public string Label { get; set; }
            public string Dag1 { get; set; }
            public string Dag2 { get; set; }
            public string Dag3 { get; set; }
            public string Dag4 { get; set; }
            public string Dag5 { get; set; }
            public string Dag6 { get; set; }
            public string Dag7 { get; set; }
            public int Level1 { get; set; }
            public int Level2 { get; set; }
            public int Level3 { get; set; }
            public int Level4 { get; set; }
            public int Level5 { get; set; }
            public int Level6 { get; set; }
            public int Level7 { get; set; }


        }

        public async Task<string> GetSMPday(string datestring, string apikey, string listtype)
        {
            string url = "https://app.slimmemeterportal.nl/userapi/v1/connections/" + this.DeviceID.Trim() + "/usage/" + datestring;

            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Properties.Add("Content-Type", "application/json");
            request.Headers.Add("API-key", apikey);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // Parse the response body

                switch (this.DeviceType)
                {
                    case "gas":
                        GasMetingen gasVerbruik = new GasMetingen
                        {
                            DeviceID = this.DeviceID,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            MetingLijst = JsonConvert.DeserializeObject<GasDagMeting>(responseString)
                        };
                        if (listtype == "Usage")
                        {
                            this.UsageGasMetingen.Add(gasVerbruik);
                        }
                        else
                        {
                            this.ReferenceGasMetingen.Add(gasVerbruik);
                        }
                            break;

                    case "elektriciteit":
                        StroomMetingen stroomVerbruik = new StroomMetingen
                        {
                            DeviceID = this.DeviceID,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            MetingLijst = JsonConvert.DeserializeObject<StroomDagMeting>(responseString)
                        };
                        if (listtype == "Usage")
                        {
                            this.UsageStroomMetingen.Add(stroomVerbruik);
                        }
                        else
                        {
                            this.ReferenceStroomMetingen.Add(stroomVerbruik);
                        }
                        break;

                    default:
                        throw new Exception("Unknown device type " + this.DeviceType);

                }

                return "Ok";
            }
            else
            {
                return "Nok";
            }


        }       
        public void GasConsolidatie(string listtype)
        {
            List<GasMetingen> listIN;
            if (listtype == "Usage")
            {
                listIN = this.UsageGasMetingen;  
            }
            else
            {
                listIN = this.ReferenceGasMetingen;
            }     

            foreach (GasMetingen gasverbruik in listIN)
            {
                DagVerbruik dagVerbruik = new DagVerbruik
                {
                    VerbruiksDatum = gasverbruik.VerbruiksDatum,
                    VerbruiksType = "Gas",
                    DeviceID = gasverbruik.DeviceID
                };

                int verbruiksdag = gasverbruik.VerbruiksDatum.Day;
                int uurnummer = 0;
                int aantalmetingen = 0;
                decimal uurverbruik = 0;
                string uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                DateTime currentdate = DateTime.MinValue;

                foreach (GasUsage gasusage in gasverbruik.MetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(gasusage.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    int currenthour = currentdate.Hour;
                    int currentday = currentdate.Day;
                    if (currentday != verbruiksdag)
                    {
                        currenthour = 24;
                    }
                    int d = Int32.Parse(gasusage.delivery.Replace(",", "").Replace(".", ""));
                    decimal ddec = d;
                    decimal delivery = ddec / 100;
                    uurverbruik += delivery;
                    aantalmetingen++;
                    if (currenthour == uurnummer)
                    {
                        continue;
                    }

                    for (int i = uurnummer; i < currenthour; i++)
                    {
                        if ((uurnummer + 1) == currenthour)
                        {
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                UurVerbruik = uurverbruik,
                                AantalMetingen = aantalmetingen
                            };
                            dagVerbruik.UurLijst.Add(uurVerbruikEntry);

                            uurnummer++;
                            aantalmetingen = 0;
                            uurverbruik = 0;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                        else
                        {
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                UurVerbruik = 0,
                                AantalMetingen = 0
                            };
                            dagVerbruik.UurLijst.Add(uurVerbruikEntry);
                            uurnummer++;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                    }
                }
                if (uurnummer < 24)
                {
                    for (int i = uurnummer; i < 24; i++)
                    {
                        uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                        {
                            UurLabel = uurlabel,
                            VerbruiksTijdstip = currentdate,
                            UurNummer = uurnummer,
                            UurVerbruik = 0,
                            AantalMetingen = 0
                        };
                        dagVerbruik.UurLijst.Add(uurVerbruikEntry);
                        uurnummer++;

                    }

                }
                if (listtype == "Usage")
                {
                    this.UurVerbruik.Add(dagVerbruik);
                }
                else
                {
                    this.UurReference.Add(dagVerbruik);
                }                
            }

            return;
        }
        public void StroomConsolidatie(string listtype)
        {
            List<StroomMetingen> listIN;
            if (listtype == "Usage")
            {
                listIN = this.UsageStroomMetingen;
            }
            else
            {
                listIN = this.ReferenceStroomMetingen;
            }

            foreach (StroomMetingen stroomverbruik in listIN)
            {
                DagVerbruik dagVerbruik = new DagVerbruik
                {
                    VerbruiksDatum = stroomverbruik.VerbruiksDatum,
                    VerbruiksType = "Stroom",
                    DeviceID = stroomverbruik.DeviceID
                };

                int verbruiksdag = stroomverbruik.VerbruiksDatum.Day;
                int uurnummer = 0;
                int aantalmetingen = 0;
                decimal uurverbruik = 0;
                string uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                DateTime currentdate = DateTime.MinValue;

                foreach (StroomUsage stroomusage in stroomverbruik.MetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(stroomusage.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    int currenthour = currentdate.Hour;
                    int currentday = currentdate.Day;
                    int d1;
                    int d2;
                    if (currentday != verbruiksdag)
                    {
                        currenthour = 24;
                    }
                    if (string.IsNullOrEmpty(stroomusage.delivery_high))
                    {
                        d1 = 0;
                    }
                    else
                    {
                        d1 = Int32.Parse(stroomusage.delivery_high.Replace(",", "").Replace(".", ""));
                    }
                    if (string.IsNullOrEmpty(stroomusage.delivery_low))
                    {
                        d2 = 0;
                    }
                    else
                    {
                        d2 = Int32.Parse(stroomusage.delivery_low.Replace(",", "").Replace(".", ""));
                    }
                    decimal ddec1 = d1;
                    decimal ddec2 = d2;
                    decimal delivery = (ddec1 + ddec2) / 100;
                    uurverbruik += delivery;
                    aantalmetingen++;
                    if (currenthour == uurnummer)
                    {
                        continue;
                    }

                    for (int i = uurnummer; i < currenthour; i++)
                    {
                        if ((uurnummer + 1) == currenthour)
                        {
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                UurVerbruik = uurverbruik,
                                AantalMetingen = aantalmetingen
                            };
                            dagVerbruik.UurLijst.Add(uurVerbruikEntry);

                            uurnummer++;
                            aantalmetingen = 0;
                            uurverbruik = 0;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                        else
                        {
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                UurVerbruik = 0,
                                AantalMetingen = 0
                            };
                            dagVerbruik.UurLijst.Add(uurVerbruikEntry);
                            uurnummer++;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                    }
                }
                if (uurnummer < 24)
                {
                    for (int i = uurnummer; i < 24; i++)
                    {
                        uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                        {
                            UurLabel = uurlabel,
                            VerbruiksTijdstip = currentdate,
                            UurNummer = uurnummer,
                            UurVerbruik = 0,
                            AantalMetingen = 0
                        };
                        dagVerbruik.UurLijst.Add(uurVerbruikEntry);
                        uurnummer++;

                    }

                }
                if (listtype == "Usage")
                {
                    this.UurVerbruik.Add(dagVerbruik);
                }
                else
                {
                    this.UurReference.Add(dagVerbruik);
                }
            }

            return;
        }

        public void GetStats()
        {
            
            for (int i = 0; i < 24; i++)
            {
                UurStats t = new UurStats();
                this.Statistieken.UurStatsList.Add(t);
            }

            // First fill waarnemingenlijst
            foreach (DagVerbruik dagverbruik in this.UurReference)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (dagverbruik.UurLijst[i].UurNummer != i)
                    {
                        throw new Exception("Uurnummer matcht niet in GETSTATS");
                    }

                    this.Statistieken.UurStatsList[i].UurLabel = dagverbruik.UurLijst[i].UurLabel;
                    int N = dagverbruik.UurLijst[i].AantalMetingen;
                    if (N != 0)
                    {
                        this.Statistieken.UurStatsList[i].AantalWaarnemingen++;
                    }
                    this.Statistieken.UurStatsList[i].Uurwaarden.Add(dagverbruik.UurLijst[i].UurVerbruik);
                }

                this.Statistieken.DagStats.Dagwaarden.Add(dagverbruik.TotaalperDag);
                this.Statistieken.DagStats.Datums.Add(dagverbruik.VerbruiksDatum);

            }

            // Bereken percentielen per uur en per dag
            for (int i = 0; i < 24; i++)
            {
                this.Statistieken.UurStatsList[i].StatNumbers = GetNumbers(this.Statistieken.UurStatsList[i].Uurwaarden);
            }
            this.Statistieken.DagStats.StatNumbers = GetNumbers(this.Statistieken.DagStats.Dagwaarden);

            return;
        }

        public StatNumbers GetNumbers(List<decimal> getallenlijst)
        {
            List<decimal> sortedlist = getallenlijst.OrderBy(number => number).ToList();
            decimal minimum = 99999;
            decimal maximum = -1;
            decimal totaal = 0;
            int aantal = sortedlist.Count;
            foreach (decimal number in getallenlijst)
            {
                totaal += number;
                if (number < minimum)
                {
                    minimum = number;
                }
                if (number > maximum)
                {
                    maximum = number;
                }
            }
            decimal gemiddelde = totaal / aantal;

            decimal a = aantal;

            decimal perc00 = sortedlist[0];

            decimal p20dec = 20M * (a - 1M) / 100M;
            if (p20dec < 0) { p20dec = 0; }
            if (p20dec >= a - 1M) { p20dec = aantal - 1; }
            int p20down = Convert.ToInt32(Math.Truncate(p20dec));
            int p20up = p20down + 1;
            if (p20up > aantal - 1) { p20up = aantal - 1; }
            decimal perc20 = sortedlist[p20down] + ((sortedlist[p20up] - sortedlist[p20down]) * (0.5M));

            decimal p40dec = 40M * (a - 1M) / 100M;
            if (p40dec < 0) { p40dec = 0; }
            if (p40dec >= a - 1M) { p40dec = aantal - 1; }
            int p40down = Convert.ToInt32(Math.Truncate(p40dec));
            int p40up = p40down + 1;
            if (p40up > aantal - 1) { p40up = aantal - 1; }
            decimal perc40 = sortedlist[p40down] + ((sortedlist[p40up] - sortedlist[p40down]) * (0.5M));

            decimal p60dec = 60M * (a - 1M) / 100M;
            if (p60dec < 0) { p60dec = 0; }
            if (p60dec >= a - 1M) { p60dec = aantal - 1; }
            int p60down = Convert.ToInt32(Math.Truncate(p60dec));
            int p60up = p60down + 1;
            if (p60up > aantal - 1) { p60up = aantal - 1; }
            decimal perc60 = sortedlist[p60down] + ((sortedlist[p60up] - sortedlist[p60down]) * (0.5M));

            decimal p80dec = 80M * (a - 1M) / 100M;
            if (p80dec < 0) { p80dec = 0; }
            if (p80dec >= a - 1M) { p80dec = aantal - 1; }
            int p80down = Convert.ToInt32(Math.Truncate(p80dec));
            int p80up = p80down + 1;
            if (p80up > aantal - 1) { p80up = aantal - 1; }
            decimal perc80 = sortedlist[p80down] + ((sortedlist[p80up] - sortedlist[p80down]) * (0.5M));

            decimal perc100 = sortedlist[aantal - 1];

            StatNumbers statnumbers = new StatNumbers
            {
                Min = minimum,
                Max = maximum,
                Mean = gemiddelde,
                Perc000 = perc00,
                Perc020 = perc20,
                Perc040 = perc40,
                Perc060 = perc60,
                Perc080 = perc80,
                Perc100 = perc100
            };

            return statnumbers;
        }

        public void Create_DagRapport()
        {
            // Titles
            RPT_line line = new RPT_line
            {
                N = "N",
                Min = "Min",
                Mean = "Mean",
                Max = "Max",
                Label = "Uur",
                Dag1 = this.UurVerbruik[0].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag2 = this.UurVerbruik[1].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag3 = this.UurVerbruik[2].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag4 = this.UurVerbruik[3].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag5 = this.UurVerbruik[4].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag6 = this.UurVerbruik[5].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag7 = this.UurVerbruik[6].VerbruiksDatum.ToString("yyyy-MM-dd")
            };
            this.DagRapport.Add(line);

            for (int i = 0; i < 24; i++)
            {
                RPT_line lined = new RPT_line
                {
                    N = Statistieken.UurStatsList[i].AantalWaarnemingen.ToString("D2"),
                    Min = Statistieken.UurStatsList[i].StatNumbers.Min.ToString("N2"),
                    Mean = Statistieken.UurStatsList[i].StatNumbers.Mean.ToString("N2"),
                    Max = Statistieken.UurStatsList[i].StatNumbers.Max.ToString("N2"),
                    Label = Statistieken.UurStatsList[i].UurLabel,
                    Dag1 = this.UurVerbruik[0].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag2 = this.UurVerbruik[1].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag3 = this.UurVerbruik[2].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag4 = this.UurVerbruik[3].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag5 = this.UurVerbruik[4].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag6 = this.UurVerbruik[5].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag7 = this.UurVerbruik[6].UurLijst[i].UurVerbruik.ToString("N2"),
                    Level1 = Statistieken.GetLevel(this.UurVerbruik[0].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level2 = Statistieken.GetLevel(this.UurVerbruik[1].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level3 = Statistieken.GetLevel(this.UurVerbruik[2].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level4 = Statistieken.GetLevel(this.UurVerbruik[3].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level5 = Statistieken.GetLevel(this.UurVerbruik[4].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level6 = Statistieken.GetLevel(this.UurVerbruik[5].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level7 = Statistieken.GetLevel(this.UurVerbruik[6].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers)
                };
                this.DagRapport.Add(lined);

            }
            RPT_line linex = new RPT_line
            {
                N = Statistieken.DagStats.AantalWaarnemingen.ToString("D2"),
                Min = Statistieken.DagStats.StatNumbers.Min.ToString("N2"),
                Mean = Statistieken.DagStats.StatNumbers.Mean.ToString("N2"),
                Max = Statistieken.DagStats.StatNumbers.Max.ToString("N2"),
                Label = "Dag Totaal",
                Dag1 = this.UurVerbruik[0].TotaalperDag.ToString("N2"),
                Dag2 = this.UurVerbruik[1].TotaalperDag.ToString("N2"),
                Dag3 = this.UurVerbruik[2].TotaalperDag.ToString("N2"),
                Dag4 = this.UurVerbruik[3].TotaalperDag.ToString("N2"),
                Dag5 = this.UurVerbruik[4].TotaalperDag.ToString("N2"),
                Dag6 = this.UurVerbruik[5].TotaalperDag.ToString("N2"),
                Dag7 = this.UurVerbruik[6].TotaalperDag.ToString("N2"),
                Level1 = Statistieken.GetLevel(this.UurVerbruik[0].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level2 = Statistieken.GetLevel(this.UurVerbruik[1].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level3 = Statistieken.GetLevel(this.UurVerbruik[2].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level4 = Statistieken.GetLevel(this.UurVerbruik[3].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level5 = Statistieken.GetLevel(this.UurVerbruik[4].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level6 = Statistieken.GetLevel(this.UurVerbruik[5].TotaalperDag, Statistieken.DagStats.StatNumbers),
                Level7 = Statistieken.GetLevel(this.UurVerbruik[6].TotaalperDag, Statistieken.DagStats.StatNumbers)
            };
            this.DagRapport.Add(linex);

            return;
        }
    }
}
