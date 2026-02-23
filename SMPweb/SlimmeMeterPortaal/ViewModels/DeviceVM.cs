using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;

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

        [DisplayName("Laatste datum met data")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime LastDateWithData { get; set; }

        [DisplayName("Rapportage datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime RapportageDatum { get; set; }

        public string Eenheid
        {
            get
            {
                if (this.DeviceType == "gas")
                {
                    return "m3";
                }
                else
                {
                    return "KwH";
                }
            }
        }

        public string ReportType
        {
            get
            { return this.DeviceType[0].ToString().ToUpper() + this.DeviceType.Substring(1); }        
        }

        [DisplayName("Meter start datum")]
        public System.DateTime Startdate { get; set; }

        [DisplayName("Meter eind datum")]
        public Nullable<System.DateTime> Enddate { get; set; }

        // Lijsten met ruwe meter data
        public List<GasMetingen> ReferenceGasMetingen = new List<GasMetingen>();
        public List<StroomMetingen> ReferenceStroomMetingen = new List<StroomMetingen>();
        public List<GasMetingen> UsageGasMetingen = new List<GasMetingen>();
        public List<StroomMetingen> UsageStroomMetingen = new List<StroomMetingen>();
        public List<GasMetingen> MaandGasMetingen = new List<GasMetingen>();
        public List<StroomMetingen> MaandStroomMetingen = new List<StroomMetingen>();

        // Lijsten met per uur geconsolideerde data 
        public List<DagVerbruik> UurVerbruik = new List<DagVerbruik>();
        public List<DagVerbruik> UurReference = new List<DagVerbruik>();
        
        // Lijsten met statistieken 
        public Stats Statistieken = new Stats();

        // Lijsten met maandverbruik per jaar
        public List<MaandVerbruik> MaandVerbruiken = new List<MaandVerbruik>() ;
        public MaandStats MaandStats = new MaandStats();
        private readonly string[] MaandArray = { "Jan", "Feb", "Mrt", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec" };
        public string LastMonthDate {  get; set; }
        public decimal LastMonthForecast { get; set; }  
        public int LastMonthLevel { get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null);
                int maand = lm.Month;
                return this.MaandStats.GetLevel(this.LastMonthForecast, this.MaandStats.MaandGegevens[maand-1].MaandStatistiek);
            } 
        }

        public string LastMonthName { get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null).AddDays(-1);
                int maand = lm.Month;
                return this.MaandArray[maand - 1];
            } 
        }

        // API call statistics
        public int Total_API_Calls { get; set; } = 0;
        public int Total_API_Calls_Success { get; set; } = 0;
        public int Total_API_Calls_Retried { get; set; } = 0;
        public int Total_API_Calls_Failed { get; set; } = 0;
        public int Total_Retries { get; set; } = 0;


        public List<RPT_line> DagRapport = new List<RPT_line>();
        public class RPT_line
        {
            public string N { get; set; }
            public string Min { get; set; }
            public string Mean { get; set; }
            public string Max { get; set; }
            public string Label { get; set; }
            public string Eenheid { get; set; }
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

        public List<RPT_Mline> MaandRapport = new List<RPT_Mline>();
        public class RPT_Mline
        {
            public string Jaar { get; set; }
            public string Attribuut { get; set; }
            public string Eenheid { get; set; }
            public string Totaal { get; set; }
            public string Maand01 { get; set; }
            public string Maand02 { get; set; }
            public string Maand03 { get; set; }
            public string Maand04 { get; set; }
            public string Maand05 { get; set; }
            public string Maand06 { get; set; }
            public string Maand07 { get; set; }
            public string Maand08 { get; set; }
            public string Maand09 { get; set; }
            public string Maand10 { get; set; }
            public string Maand11 { get; set; }
            public string Maand12 { get; set; }
            public int Level01 { get; set; }
            public int Level02 { get; set; }
            public int Level03 { get; set; }
            public int Level04 { get; set; }
            public int Level05 { get; set; }
            public int Level06 { get; set; }
            public int Level07 { get; set; }
            public int Level08 { get; set; }
            public int Level09 { get; set; }
            public int Level10 { get; set; }
            public int Level11 { get; set; }
            public int Level12 { get; set; }                   
        }

        public List<Meterstand> Meterstanden = new List<Meterstand>();
        public class Meterstand
        {
            public string Jaar { get; set; }
            public string Eenheid { get; set; }
            public string Maand01 { get; set; }
            public string Maand02 { get; set; }
            public string Maand03 { get; set; }
            public string Maand04 { get; set; }
            public string Maand05 { get; set; }
            public string Maand06 { get; set; }
            public string Maand07 { get; set; }
            public string Maand08 { get; set; }
            public string Maand09 { get; set; }
            public string Maand10 { get; set; }
            public string Maand11 { get; set; }
            public string Maand12 { get; set; }
        }

        public async Task<string> GetSMPday(string datestring, string apikey, string listtype)
        {
            string url = "https://app.slimmemeterportal.nl/userapi/v1/connections/" + this.DeviceID.Trim() + "/usage/" + datestring;

            int retrycount = 0;
            string result;
            var responseString = "";
            do
            {
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
                    result = "Ok";                    
                    responseString = await response.Content.ReadAsStringAsync();
                }
                else
                {                    
                    result = "Nok";
                    retrycount += 1;
                    Thread.Sleep(10000);
                }                  
                
            }
            while ((result != "Ok") && (retrycount < 10));

            // Statistics
            this.Total_API_Calls += 1;
            if (result == "Ok")
            {
                if (retrycount == 0)
                {
                    this.Total_API_Calls_Success += 1;
                }
                else
                {
                    this.Total_API_Calls_Retried += 1;
                    this.Total_Retries += retrycount; 
                }

            }
            else
            {
                Total_API_Calls_Failed += 1;
                this.Total_Retries += retrycount;
            }
            
            if (result == "Ok")
            {                
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
                        switch (listtype)
                        {
                            case "Lastdate":
                                if (gasVerbruik.MetingLijst.usages.Count > 0) { result = "Last"; };
                                break;
                            case "Usage":
                                this.UsageGasMetingen.Add(gasVerbruik);
                                break;
                            case "Reference":
                                this.ReferenceGasMetingen.Add(gasVerbruik);
                                break;
                            case "Month":
                                this.MaandGasMetingen.Add(gasVerbruik);
                                break;
                        }
                        break;

                    case "elektriciteit":
                        StroomMetingen stroomVerbruik = new StroomMetingen
                        {
                            DeviceID = this.DeviceID,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            MetingLijst = JsonConvert.DeserializeObject<StroomDagMeting>(responseString)
                        };
                        switch (listtype)
                        {
                            case "Lastdate":
                                if (stroomVerbruik.MetingLijst.usages.Count > 0) { result = "Last"; };
                                break;
                            case "Usage":
                                this.UsageStroomMetingen.Add(stroomVerbruik);
                                break;
                            case "Reference":
                                this.ReferenceStroomMetingen.Add(stroomVerbruik);
                                break;
                            case "Month":
                                this.MaandStroomMetingen.Add(stroomVerbruik);
                                break;                                
                        }
                        break;

                    default:
                        throw new Exception("Unknown device type " + this.DeviceType);

                }
                
            }
            return result;


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
                string utc = ""; 

                foreach (GasUsage gasusage in gasverbruik.MetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(gasusage.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    utc = gasusage.time.Substring(20,3);
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
                        // Just keep on filling the hour usage entry while in this hour
                        continue;
                    }

                    for (int i = uurnummer; i < currenthour; i++)
                    {
                        if ((uurnummer + 1) == currenthour)
                        // Next hour starts here, so output current hour
                        {
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
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
                            // Apparantly we are missing an hour, so just output a zero hour usage
                            UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
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
                    // Hour should be 24 by now. If not, just add enough zero entries
                    for (int i = uurnummer; i < 24; i++)
                    {
                        uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        UurVerbruikEntry uurVerbruikEntry = new UurVerbruikEntry()
                        {
                            UurLabel = uurlabel,
                            UTC = utc,
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
                string utc = "";

                foreach (StroomUsage stroomusage in stroomverbruik.MetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(stroomusage.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    utc = stroomusage.time.Substring(20, 3);
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
                                UTC = utc,
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
                                UTC = utc,
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
                            UTC = utc,
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
                Eenheid = "Eenheid",
                Dag1 = this.UurVerbruik[0].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag2 = this.UurVerbruik[1].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag3 = this.UurVerbruik[2].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag4 = this.UurVerbruik[3].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag5 = this.UurVerbruik[4].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag6 = this.UurVerbruik[5].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag7 = this.UurVerbruik[6].VerbruiksDatum.ToString("yyyy-MM-dd")
            };
            this.DagRapport.Add(line);

            RPT_line lineu = new RPT_line
            {
                N = "",
                Min = "",
                Mean = "",
                Max = "",
                Label = "",
                Eenheid = "",
                Dag1 = this.UurVerbruik[0].UTC,
                Dag2 = this.UurVerbruik[1].UTC,
                Dag3 = this.UurVerbruik[2].UTC,
                Dag4 = this.UurVerbruik[3].UTC,
                Dag5 = this.UurVerbruik[4].UTC,
                Dag6 = this.UurVerbruik[5].UTC,
                Dag7 = this.UurVerbruik[6].UTC
            };
            this.DagRapport.Add(lineu);

            for (int i = 0; i < 24; i++)
            {
                RPT_line lined = new RPT_line
                {
                    N = Statistieken.UurStatsList[i].AantalWaarnemingen.ToString("D2"),
                    Min = Statistieken.UurStatsList[i].StatNumbers.Min.ToString("N2"),
                    Mean = Statistieken.UurStatsList[i].StatNumbers.Mean.ToString("N2"),
                    Max = Statistieken.UurStatsList[i].StatNumbers.Max.ToString("N2"),
                    Label = Statistieken.UurStatsList[i].UurLabel,
                    Eenheid = this.Eenheid,
                    Dag1 = this.UurVerbruik[0].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[0].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag2 = this.UurVerbruik[1].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[1].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag3 = this.UurVerbruik[2].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[2].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag4 = this.UurVerbruik[3].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[3].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag5 = this.UurVerbruik[4].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[4].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag6 = this.UurVerbruik[5].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[5].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag7 = this.UurVerbruik[6].UurLijst[i].AantalMetingen == 0 ? "n/a" : this.UurVerbruik[6].UurLijst[i].UurVerbruik.ToString("N2"),
                    Level1 = this.UurVerbruik[0].UurLijst[i].AantalMetingen == 0 ? 9 :
                            Statistieken.GetLevel(this.UurVerbruik[0].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level2 = this.UurVerbruik[0].UurLijst[i].AantalMetingen == 0 ? 9 :
                            Statistieken.GetLevel(this.UurVerbruik[1].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level3 = this.UurVerbruik[2].UurLijst[i].AantalMetingen == 0 ? 9 : Statistieken.GetLevel(this.UurVerbruik[2].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level4 = this.UurVerbruik[3].UurLijst[i].AantalMetingen == 0 ? 9 : Statistieken.GetLevel(this.UurVerbruik[3].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level5 = this.UurVerbruik[4].UurLijst[i].AantalMetingen == 0 ? 9 : Statistieken.GetLevel(this.UurVerbruik[4].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level6 = this.UurVerbruik[5].UurLijst[i].AantalMetingen == 0 ? 9 : Statistieken.GetLevel(this.UurVerbruik[5].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers),
                    Level7 = this.UurVerbruik[6].UurLijst[i].AantalMetingen == 0 ? 9 : Statistieken.GetLevel(this.UurVerbruik[6].UurLijst[i].UurVerbruik, Statistieken.UurStatsList[i].StatNumbers)
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
                Eenheid = this.Eenheid,
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

        public async Task<string> GetSMPmonth(int year, string apikey)
        {
            string datestring;
            
            for (int mo = 1; mo <= 12; mo++)
            {
                datestring = "01-" + mo.ToString("D2") + "-" + year.ToString("D4");

                Task<string> longRunningTask = this.GetSMPday(datestring, apikey, "Month");
                string result = await longRunningTask;

                DateTime ldate = this.RapportageDatum;
                if ((ldate.Year == year) && (ldate.Month == mo)) {

                    DateTime testdate = ldate.AddDays(1); // Add one day to INCLUDE the usage of the last day!
                    
                    if (testdate > this.LastDateWithData)
                    {
                        // if no data available, leave ldate as it is and adjust report date
                        this.RapportageDatum = this.RapportageDatum.AddDays(-1);
                    }
                    else
                    {
                        // if data available. adjust ldate
                        ldate = testdate;
                    }
                    datestring = ldate.ToString("dd-MM-yyyy");
                    this.LastMonthDate = datestring;
                    Task<string> longRunningTask2 = this.GetSMPday(datestring, apikey, "Month");
                    string result2 = await longRunningTask2;

                    // this was the last date to proces
                    break;
                }               
            }
            return "Ok";

        }

        public void GasMaandCijfers()
        {                       
            int maandnr;
            int currentyear = 0;
            string maandlabel;
            decimal startreading = 0;
            decimal endreading;
            decimal meterstandtotaal = 0;
                        
            int startyear = this.MaandGasMetingen[0].VerbruiksDatum.Year;
            int endyear = this.RapportageDatum.Year;

            for (int y = startyear; y <= endyear; y++)
            {
                MaandVerbruik maandverbruik = new MaandVerbruik();
                bool newyear = true;
                foreach (GasMetingen GasMetingen in this.MaandGasMetingen)
                {
                    if (GasMetingen.VerbruiksDatum.Year < y) { continue; }

                    maandnr = GasMetingen.VerbruiksDatum.Month;

                    if (newyear)
                    {
                        if (maandnr != 1)
                        {
                            throw new Exception("Maandoverzicht kan niet starten met een jaar dat start met maand " + maandnr.ToString());
                        }
                        else
                        {                            
                            // eerste maand van het jaar
                            maandverbruik.DeviceID = GasMetingen.DeviceID;
                            maandverbruik.Jaar = GasMetingen.VerbruiksDatum.Year;
                            maandverbruik.VerbruiksType = "gas";

                            string strt = GasMetingen.MetingLijst.usages[0].delivery_reading; // meterstand
                            string deliv = GasMetingen.MetingLijst.usages[0].delivery;

                            int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                            decimal dstart = istart;

                            int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                            decimal ddeliv = ideliv;

                            // de eerste entry van de dag bevat de meterstand aan het einde van het eerste uur. 
                            // de delivery eraf trekken geeft de stand om 00:00 uur
                            startreading = (dstart - ddeliv) / 100; // start stand van het jaar 

                            currentyear = GasMetingen.VerbruiksDatum.Year;

                            newyear = false;

                            // Get meterstanden

                            meterstandtotaal = startreading;

                        }
                    }
                    else
                    {
                        if (GasMetingen.MetingLijst.usages.Count == 0) 
                        {
                            throw new Exception("Lege meting: jaar = " + currentyear.ToString() + " maand = " + maandnr.ToString());
                        }

                        string strt = GasMetingen.MetingLijst.usages[0].delivery_reading;
                        string deliv = GasMetingen.MetingLijst.usages[0].delivery;

                        int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                        decimal dstart = istart;

                        int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                        decimal ddeliv = ideliv;

                        endreading = (dstart - ddeliv) / 100;

                        decimal monthusage = endreading - startreading;
                        
                        if (GasMetingen.VerbruiksDatum.Day == 1)
                        {
                            maandnr = GasMetingen.VerbruiksDatum.Month - 1;
                            if (maandnr == 0) { maandnr = 12; }

                        }
                        else
                        {
                            maandnr = GasMetingen.VerbruiksDatum.Month;
                            
                        }     
                        maandlabel = this.MaandArray[maandnr - 1];
                        MaandCijfer maandCijfer = new MaandCijfer
                        {
                            MaandLabel = maandlabel,
                            MaandNummer = maandnr,
                            Cijfer = monthusage,
                            MeterstandTotaal = meterstandtotaal,
                            MeterstandLaagTarief = 0,
                            MeterstandNormaalTarief = meterstandtotaal
                        };
                        maandverbruik.MaandCijfers.Add(maandCijfer);

                        startreading = endreading;

                        meterstandtotaal = startreading;

                        if (currentyear != GasMetingen.VerbruiksDatum.Year)
                        {
                            this.MaandVerbruiken.Add(maandverbruik);
                            break;
                        }

                        
                    }
                }
                if (currentyear == endyear)
                {
                    // Add last (incomplete) year
                    int q = maandverbruik.MaandCijfers.Count;
                    int maxdays = DateTime.DaysInMonth(currentyear, maandverbruik.MaandCijfers[q - 1].MaandNummer);
                    DateTime lastmonth = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null);
                    int nrofdays = lastmonth.Day - 1;

                    // determine forecast of last - incomplete - month 
                    // if reportimg goes tot end of moth, yhe last measurement is in the next (empty) month, so adjust 
                    if (nrofdays == 0)
                    {
                        lastmonth = lastmonth.AddDays(-1);
                        nrofdays = lastmonth.Day;
                        this.LastMonthDate = lastmonth.ToString("dd-MM-yyyy");
                    }
                    
                    this.LastMonthForecast = maandverbruik.MaandCijfers[q - 1].Cijfer * maxdays / nrofdays;
                    
                    // Add dummy entries for months that lay in the future
                    for (int j = q; j <= 12; j++)
                    {
                        MaandCijfer dummy = new MaandCijfer
                        {
                            MaandLabel = this.MaandArray[j - 1],
                            MaandNummer = j,
                            Cijfer = 0
                        };
                        maandverbruik.MaandCijfers.Add(dummy);
                    }
                    this.MaandVerbruiken.Add(maandverbruik);
                }
            } 
        }
        public void StroomMaandCijfers()
        {
            int maandnr;
            int currentyear = 0;
            string maandlabel;
            decimal startreading = 0;
            decimal endreading;
            decimal meterstandtotaal = 0;
            
            int startyear = this.MaandStroomMetingen[0].VerbruiksDatum.Year;
            int endyear = this.RapportageDatum.Year;

            for (int y = startyear; y <= endyear; y++)
            {
                MaandVerbruik maandverbruik = new MaandVerbruik();
                bool newyear = true;
                foreach (StroomMetingen StroomMetingen in this.MaandStroomMetingen)
                {
                    if (StroomMetingen.VerbruiksDatum.Year < y) { continue; }

                    maandnr = StroomMetingen.VerbruiksDatum.Month;

                    if (newyear)
                    {
                        if (maandnr != 1)
                        {
                            throw new Exception("Maandoverzicht kan niet starten met een jaar dat start met maand " + maandnr.ToString());
                        }
                        else
                        {
                            // eerste maand van het jaar
                            maandverbruik.DeviceID = StroomMetingen.DeviceID;
                            maandverbruik.Jaar = StroomMetingen.VerbruiksDatum.Year;
                            maandverbruik.VerbruiksType = "elektriciteit";

                            string strt = StroomMetingen.MetingLijst.usages[0].delivery_reading_combined; // meterstand
                            string deliv = StroomMetingen.MetingLijst.usages[0].delivery_high;
                            if (string.IsNullOrEmpty(deliv)) {
                                deliv = StroomMetingen.MetingLijst.usages[0].delivery_low;
                            }

                            int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                            decimal dstart = istart;

                            int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                            decimal ddeliv = ideliv;

                            // de eerste entry van de dag bevat de meterstand aan het einde van het eerste uur. 
                            // de delivery eraf trekken geeft de stand om 00:00 uur
                            startreading = (dstart - ddeliv) / 100; // start stand van het jaar 

                            currentyear = StroomMetingen.VerbruiksDatum.Year;

                            newyear = false;

                            // Get meterstanden

                            meterstandtotaal = startreading;
                                                                                   
                            
                        }
                    }
                    else
                    {
                        if (StroomMetingen.MetingLijst.usages.Count == 0)
                        {
                            throw new Exception("Lege meting: jaar = " + currentyear.ToString() + " maand = " + maandnr.ToString());
                        }
                        
                        string strt = StroomMetingen.MetingLijst.usages[0].delivery_reading_combined;
                        string deliv = StroomMetingen.MetingLijst.usages[0].delivery_high;
                        if (string.IsNullOrEmpty(deliv)) {
                            deliv = StroomMetingen.MetingLijst.usages[0].delivery_low;
                        }

                        int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                        decimal dstart = istart;

                        int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                        decimal ddeliv = ideliv;

                        endreading = (dstart - ddeliv) / 100;

                        decimal monthusage = endreading - startreading;                       
                                                
                        if (StroomMetingen.VerbruiksDatum.Day == 1)
                        {
                            maandnr = StroomMetingen.VerbruiksDatum.Month - 1;
                        }
                       
                        if (maandnr == 0) { maandnr = 12; }
                        maandlabel = this.MaandArray[maandnr - 1];
                        MaandCijfer maandCijfer = new MaandCijfer
                        {
                            MaandLabel = maandlabel,
                            MaandNummer = maandnr,
                            Cijfer = monthusage,
                            MeterstandTotaal = meterstandtotaal,
                            MeterstandNormaalTarief = meterstandtotaal,
                            MeterstandLaagTarief = 0
                        };

                        maandverbruik.MaandCijfers.Add(maandCijfer);
                        startreading = endreading;

                        // Get meterstanden
                        meterstandtotaal = startreading;                       

                        if (currentyear != StroomMetingen.VerbruiksDatum.Year)
                        {
                            this.MaandVerbruiken.Add(maandverbruik);                           
                            break;
                        }                      

                    }
                }
                if (currentyear == endyear)
                {
                    // Add last (incomplete) year
                    int q = maandverbruik.MaandCijfers.Count;
                    int maxdays = DateTime.DaysInMonth(currentyear, maandverbruik.MaandCijfers[q - 1].MaandNummer);
                    DateTime lastmonth = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null);
                    int nrofdays = lastmonth.Day - 1;

                    // determine forecast of last - incomplete - month 
                    // if reportimg goes tot end of moth, yhe last measurement is in the next (empty) month, so adjust 
                    if (nrofdays == 0)
                    {
                        lastmonth = lastmonth.AddDays(-1);
                        nrofdays = lastmonth.Day;
                        this.LastMonthDate = lastmonth.ToString("dd-MM-yyyy");
                    }

                    this.LastMonthForecast = maandverbruik.MaandCijfers[q - 1].Cijfer * maxdays / nrofdays;

                    // Add dummy entries for months that lay in the future
                    for (int j = q; j <= 12; j++)
                    {
                        MaandCijfer dummy = new MaandCijfer
                        {
                            MaandLabel = this.MaandArray[j - 1],
                            MaandNummer = j,
                            Cijfer = 0
                        };
                        maandverbruik.MaandCijfers.Add(dummy);
                    }
                    this.MaandVerbruiken.Add(maandverbruik);
                }
            }

        }

        public void MonthStats ()
        {
            for (int i = 0; i < 12; i++)
            {
                MaandGegeven MaandGegeven = new MaandGegeven{
                    Maandnr = i+1,
                    MaandLabel = this.MaandArray[i],
                    AantalWaarnemingen = 0,
                };
                MaandGegeven.MaandStatistiek.Max = int.MinValue;
                MaandGegeven.MaandStatistiek.Min = int.MaxValue;
                this.MaandStats.MaandGegevens.Add(MaandGegeven);
            }

            // get statistics for each month
            for (int i = 0; i < 12; i++)
            {
                decimal sumup = 0;
                
                foreach (MaandVerbruik maandverbruik in this.MaandVerbruiken)
                {
                    if (maandverbruik.Jaar == this.RapportageDatum.Year) 
                    {                        
                        break; 
                    }
                    if ((maandverbruik.MaandCijfers[i].MaandNummer != i + 1) && (maandverbruik.Jaar != this.RapportageDatum.Year))
                    {
                        throw new Exception("Maandnummer matcht niet in MONTHSTATS");
                    }
                    this.MaandStats.MaandGegevens[i].AantalWaarnemingen++;
                    decimal c = maandverbruik.MaandCijfers[i].Cijfer;
                    sumup += c;
                    if (c > this.MaandStats.MaandGegevens[i].MaandStatistiek.Max)
                    {
                        this.MaandStats.MaandGegevens[i].MaandStatistiek.Max = c;
                    }
                    if (c < this.MaandStats.MaandGegevens[i].MaandStatistiek.Min)
                    {
                        this.MaandStats.MaandGegevens[i].MaandStatistiek.Min = c;
                    }
                }
                this.MaandStats.MaandGegevens[i].MaandStatistiek.Mean = sumup / this.MaandStats.MaandGegevens[i].AantalWaarnemingen;                
            }

            decimal Ymin = decimal.MaxValue;
            decimal Ymax = decimal.MinValue;
            decimal Ysum = 0;
            
            foreach (MaandVerbruik m in this.MaandVerbruiken)
            {
                // Referentie Jaarcijfers
                if (m.Jaar == this.RapportageDatum.Year)
                {
                    break;
                }

                this.MaandStats.JaarStats.Jaren.Add(m.Jaar);
                Ysum += m.TotaalperJaar;
                if (m.TotaalperJaar < Ymin)
                {
                    Ymin = m.TotaalperJaar;
                }
                if (m.TotaalperJaar > Ymax)
                {
                    Ymax = m.TotaalperJaar;
                }
            }
            this.MaandStats.JaarStats.MYStatistiek.Max = Ymax;
            this.MaandStats.JaarStats.MYStatistiek.Min = Ymin;
            this.MaandStats.JaarStats.MYStatistiek.Mean = Ysum / this.MaandStats.JaarStats.AantalWaarnemingen;

            return;
        }

        public void Create_MaandRapport()
        {
            // Titles
            RPT_Mline Mline = new RPT_Mline
            {
                Jaar = "Jaar",
                Attribuut = "Attribuut",
                Eenheid = "Eenheid",
                Totaal = "Totaal",
                Maand01 = this.MaandArray[0],
                Maand02 = this.MaandArray[1],
                Maand03 = this.MaandArray[2],
                Maand04 = this.MaandArray[3],
                Maand05 = this.MaandArray[4],
                Maand06 = this.MaandArray[5],
                Maand07 = this.MaandArray[6],
                Maand08 = this.MaandArray[7],
                Maand09 = this.MaandArray[8],
                Maand10 = this.MaandArray[9],
                Maand11 = this.MaandArray[10],
                Maand12 = this.MaandArray[11]                
            };
            this.MaandRapport.Add(Mline);

            RPT_Mline MlineN = new RPT_Mline
            {
                Jaar = "",
                Attribuut = "N",
                Eenheid = "",
                Totaal = this.MaandStats.JaarStats.AantalWaarnemingen.ToString("D2"),
                Maand01 = this.MaandStats.MaandGegevens[0].AantalWaarnemingen.ToString("D2"),
                Maand02 = this.MaandStats.MaandGegevens[1].AantalWaarnemingen.ToString("D2"),
                Maand03 = this.MaandStats.MaandGegevens[2].AantalWaarnemingen.ToString("D2"),
                Maand04 = this.MaandStats.MaandGegevens[3].AantalWaarnemingen.ToString("D2"),
                Maand05 = this.MaandStats.MaandGegevens[4].AantalWaarnemingen.ToString("D2"),
                Maand06 = this.MaandStats.MaandGegevens[5].AantalWaarnemingen.ToString("D2"),
                Maand07 = this.MaandStats.MaandGegevens[6].AantalWaarnemingen.ToString("D2"),
                Maand08 = this.MaandStats.MaandGegevens[7].AantalWaarnemingen.ToString("D2"),
                Maand09 = this.MaandStats.MaandGegevens[8].AantalWaarnemingen.ToString("D2"),
                Maand10 = this.MaandStats.MaandGegevens[9].AantalWaarnemingen.ToString("D2"),
                Maand11 = this.MaandStats.MaandGegevens[10].AantalWaarnemingen.ToString("D2"),
                Maand12 = this.MaandStats.MaandGegevens[11].AantalWaarnemingen.ToString("D2"),

                Level01 = 9,
                Level02 = 9,
                Level03 = 9,
                Level04 = 9,
                Level05 = 9,
                Level06 = 9,
                Level07 = 9,
                Level08 = 9,
                Level09 = 9,
                Level10 = 9,
                Level11 = 9,
                Level12 = 9
            };
            this.MaandRapport.Add(MlineN);

            RPT_Mline MlineMin = new RPT_Mline
            {
                Jaar = "",
                Attribuut = "Min",
                Eenheid = this.Eenheid,
                Totaal = this.MaandStats.JaarStats.MYStatistiek.Min.ToString("N2"),
                Maand01 = this.MaandStats.MaandGegevens[0].MaandStatistiek.Min.ToString("N2"),
                Maand02 = this.MaandStats.MaandGegevens[1].MaandStatistiek.Min.ToString("N2"),
                Maand03 = this.MaandStats.MaandGegevens[2].MaandStatistiek.Min.ToString("N2"),
                Maand04 = this.MaandStats.MaandGegevens[3].MaandStatistiek.Min.ToString("N2"),
                Maand05 = this.MaandStats.MaandGegevens[4].MaandStatistiek.Min.ToString("N2"),
                Maand06 = this.MaandStats.MaandGegevens[5].MaandStatistiek.Min.ToString("N2"),
                Maand07 = this.MaandStats.MaandGegevens[6].MaandStatistiek.Min.ToString("N2"),
                Maand08 = this.MaandStats.MaandGegevens[7].MaandStatistiek.Min.ToString("N2"),
                Maand09 = this.MaandStats.MaandGegevens[8].MaandStatistiek.Min.ToString("N2"),
                Maand10 = this.MaandStats.MaandGegevens[9].MaandStatistiek.Min.ToString("N2"),
                Maand11 = this.MaandStats.MaandGegevens[10].MaandStatistiek.Min.ToString("N2"),
                Maand12 = this.MaandStats.MaandGegevens[11].MaandStatistiek.Min.ToString("N2"),

                Level01 = 9,
                Level02 = 9,
                Level03 = 9, 
                Level04 = 9,   
                Level05 = 9,
                Level06 = 9,
                Level07 = 9,
                Level08 = 9,
                Level09 = 9,
                Level10 = 9,
                Level11 = 9,
                Level12 = 9
            };
            this.MaandRapport.Add(MlineMin);

            RPT_Mline MlineMean = new RPT_Mline
            {
                Jaar = "",
                Attribuut = "Mean",
                Eenheid = this.Eenheid,
                Totaal = this.MaandStats.JaarStats.MYStatistiek.Mean.ToString("N2"),
                Maand01 = this.MaandStats.MaandGegevens[0].MaandStatistiek.Mean.ToString("N2"),
                Maand02 = this.MaandStats.MaandGegevens[1].MaandStatistiek.Mean.ToString("N2"),
                Maand03 = this.MaandStats.MaandGegevens[2].MaandStatistiek.Mean.ToString("N2"),
                Maand04 = this.MaandStats.MaandGegevens[3].MaandStatistiek.Mean.ToString("N2"),
                Maand05 = this.MaandStats.MaandGegevens[4].MaandStatistiek.Mean.ToString("N2"),
                Maand06 = this.MaandStats.MaandGegevens[5].MaandStatistiek.Mean.ToString("N2"),
                Maand07 = this.MaandStats.MaandGegevens[6].MaandStatistiek.Mean.ToString("N2"),
                Maand08 = this.MaandStats.MaandGegevens[7].MaandStatistiek.Mean.ToString("N2"),
                Maand09 = this.MaandStats.MaandGegevens[8].MaandStatistiek.Mean.ToString("N2"),
                Maand10 = this.MaandStats.MaandGegevens[9].MaandStatistiek.Mean.ToString("N2"),
                Maand11 = this.MaandStats.MaandGegevens[10].MaandStatistiek.Mean.ToString("N2"),
                Maand12 = this.MaandStats.MaandGegevens[11].MaandStatistiek.Mean.ToString("N2"),

                Level01 = 9,
                Level02 = 9,
                Level03 = 9,
                Level04 = 9,
                Level05 = 9,
                Level06 = 9,
                Level07 = 9,
                Level08 = 9,
                Level09 = 9,
                Level10 = 9,
                Level11 = 9,
                Level12 = 9
            };
            this.MaandRapport.Add(MlineMean);

            RPT_Mline MlineMax = new RPT_Mline
            {
                Jaar = "",
                Attribuut = "Max",
                Eenheid = this.Eenheid,
                Totaal = this.MaandStats.JaarStats.MYStatistiek.Max.ToString("N2"),
                Maand01 = this.MaandStats.MaandGegevens[0].MaandStatistiek.Max.ToString("N2"),
                Maand02 = this.MaandStats.MaandGegevens[1].MaandStatistiek.Max.ToString("N2"),
                Maand03 = this.MaandStats.MaandGegevens[2].MaandStatistiek.Max.ToString("N2"),
                Maand04 = this.MaandStats.MaandGegevens[3].MaandStatistiek.Max.ToString("N2"),
                Maand05 = this.MaandStats.MaandGegevens[4].MaandStatistiek.Max.ToString("N2"),
                Maand06 = this.MaandStats.MaandGegevens[5].MaandStatistiek.Max.ToString("N2"),
                Maand07 = this.MaandStats.MaandGegevens[6].MaandStatistiek.Max.ToString("N2"),
                Maand08 = this.MaandStats.MaandGegevens[7].MaandStatistiek.Max.ToString("N2"),
                Maand09 = this.MaandStats.MaandGegevens[8].MaandStatistiek.Max.ToString("N2"),
                Maand10 = this.MaandStats.MaandGegevens[9].MaandStatistiek.Max.ToString("N2"),
                Maand11 = this.MaandStats.MaandGegevens[10].MaandStatistiek.Max.ToString("N2"),
                Maand12 = this.MaandStats.MaandGegevens[11].MaandStatistiek.Max.ToString("N2"),

                Level01 = 9,
                Level02 = 9,
                Level03 = 9,
                Level04 = 9,
                Level05 = 9,
                Level06 = 9,
                Level07 = 9,
                Level08 = 9,
                Level09 = 9,
                Level10 = 9,
                Level11 = 9,
                Level12 = 9
            };
            this.MaandRapport.Add(MlineMax);

            foreach (MaandVerbruik m in this.MaandVerbruiken)
            {
                RPT_Mline lined = new RPT_Mline
                {
                    Jaar = m.Jaar.ToString("D2"),
                    Attribuut = "",
                    Eenheid = this.Eenheid,
                    Totaal = m.TotaalperJaar.ToString("N2"),
                    
                    Maand01 = (m.MaandCijfers[0].Cijfer == 0) ? "n/a" : m.MaandCijfers[0].Cijfer.ToString("N2"),
                    Maand02 = (m.MaandCijfers[1].Cijfer == 0) ? "n/a" : m.MaandCijfers[1].Cijfer.ToString("N2"),
                    Maand03 = (m.MaandCijfers[2].Cijfer == 0) ? "n/a" : m.MaandCijfers[2].Cijfer.ToString("N2"),
                    Maand04 = (m.MaandCijfers[3].Cijfer == 0) ? "n/a" : m.MaandCijfers[3].Cijfer.ToString("N2"),
                    Maand05 = (m.MaandCijfers[4].Cijfer == 0) ? "n/a" : m.MaandCijfers[4].Cijfer.ToString("N2"),
                    Maand06 = (m.MaandCijfers[5].Cijfer == 0) ? "n/a" : m.MaandCijfers[5].Cijfer.ToString("N2"),
                    Maand07 = (m.MaandCijfers[6].Cijfer == 0) ? "n/a" : m.MaandCijfers[6].Cijfer.ToString("N2"),
                    Maand08 = (m.MaandCijfers[7].Cijfer == 0) ? "n/a" : m.MaandCijfers[7].Cijfer.ToString("N2"),
                    Maand09 = (m.MaandCijfers[8].Cijfer == 0) ? "n/a" : m.MaandCijfers[8].Cijfer.ToString("N2"),
                    Maand10 = (m.MaandCijfers[9].Cijfer == 0) ? "n/a" : m.MaandCijfers[9].Cijfer.ToString("N2"),
                    Maand11 = (m.MaandCijfers[10].Cijfer == 0) ? "n/a" : m.MaandCijfers[10].Cijfer.ToString("N2"),
                    Maand12 = (m.MaandCijfers[11].Cijfer == 0) ? "n/a" : m.MaandCijfers[11].Cijfer.ToString("N2"),

                    Level01 = (m.MaandCijfers[0].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[0].Cijfer, this.MaandStats.MaandGegevens[0].MaandStatistiek),
                    Level02 = (m.MaandCijfers[1].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[1].Cijfer, this.MaandStats.MaandGegevens[1].MaandStatistiek),
                    Level03 = (m.MaandCijfers[2].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[2].Cijfer, this.MaandStats.MaandGegevens[2].MaandStatistiek),
                    Level04 = (m.MaandCijfers[3].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[3].Cijfer, this.MaandStats.MaandGegevens[3].MaandStatistiek),
                    Level05 = (m.MaandCijfers[4].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[4].Cijfer, this.MaandStats.MaandGegevens[4].MaandStatistiek),
                    Level06 = (m.MaandCijfers[5].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[5].Cijfer, this.MaandStats.MaandGegevens[5].MaandStatistiek),
                    Level07 = (m.MaandCijfers[6].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[6].Cijfer, this.MaandStats.MaandGegevens[6].MaandStatistiek),
                    Level08 = (m.MaandCijfers[7].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[7].Cijfer, this.MaandStats.MaandGegevens[7].MaandStatistiek),
                    Level09 = (m.MaandCijfers[8].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[8].Cijfer, this.MaandStats.MaandGegevens[8].MaandStatistiek),
                    Level10 = (m.MaandCijfers[9].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[9].Cijfer, this.MaandStats.MaandGegevens[9].MaandStatistiek),
                    Level11 = (m.MaandCijfers[10].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[10].Cijfer, this.MaandStats.MaandGegevens[10].MaandStatistiek),
                    Level12 = (m.MaandCijfers[11].Cijfer == 0) ? 9 : this.MaandStats.GetLevel(m.MaandCijfers[11].Cijfer, this.MaandStats.MaandGegevens[11].MaandStatistiek),
                };
                this.MaandRapport.Add(lined);

            }

            //==========================================================================================

            Meterstand Meterstand1 = new Meterstand
            {
                Jaar = "Jaar",               
                Eenheid = "Eenheid",               
                Maand01 = this.MaandArray[0],
                Maand02 = this.MaandArray[1],
                Maand03 = this.MaandArray[2],
                Maand04 = this.MaandArray[3],
                Maand05 = this.MaandArray[4],
                Maand06 = this.MaandArray[5],
                Maand07 = this.MaandArray[6],
                Maand08 = this.MaandArray[7],
                Maand09 = this.MaandArray[8],
                Maand10 = this.MaandArray[9],
                Maand11 = this.MaandArray[10],
                Maand12 = this.MaandArray[11]
            };
            this.Meterstanden.Add(Meterstand1);

            foreach (MaandVerbruik m in this.MaandVerbruiken)
            {
                Meterstand Meterstand2 = new Meterstand
                {
                    Jaar = m.Jaar.ToString("D2"),
                    Eenheid = this.Eenheid,

                    Maand01 = m.MaandCijfers[0].MeterstandTotaal.ToString("N0"),
                    Maand02 = m.MaandCijfers[1].MeterstandTotaal.ToString("N0"),
                    Maand03 = m.MaandCijfers[2].MeterstandTotaal.ToString("N0"),
                    Maand04 = m.MaandCijfers[3].MeterstandTotaal.ToString("N0"),
                    Maand05 = m.MaandCijfers[4].MeterstandTotaal.ToString("N0"),
                    Maand06 = m.MaandCijfers[5].MeterstandTotaal.ToString("N0"),
                    Maand07 = m.MaandCijfers[6].MeterstandTotaal.ToString("N0"),
                    Maand08 = m.MaandCijfers[7].MeterstandTotaal.ToString("N0"),
                    Maand09 = m.MaandCijfers[8].MeterstandTotaal.ToString("N0"),
                    Maand10 = m.MaandCijfers[9].MeterstandTotaal.ToString("N0"),
                    Maand11 = m.MaandCijfers[10].MeterstandTotaal.ToString("N0"),
                    Maand12 = m.MaandCijfers[11].MeterstandTotaal.ToString("N0")
                };
                this.Meterstanden.Add(Meterstand2);
            }


                return;
           
        }
    }
}
