using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SlimmeMeterPortaal.ViewModels
{
    public class VerbruiksMeter 
    {
        public VerbruiksMeter(string devtype, string gas, string stroom)
        {
            this.MeterType = devtype;
            if (devtype == "elektriciteit")
            {
                if (stroom == "Y")
                { 
                    this.MeterRapportMaken = true; 
                }
                else
                {
                    this.MeterRapportMaken = false;
                }
            }
            if (devtype == "gas")
            {
                if (gas == "Y")
                {
                    this.MeterRapportMaken = true;
                }
                else
                {
                    this.MeterRapportMaken = false;
                }
            }
        }
        [DisplayName("Verbruiksmeter type")]
        public string MeterType { get; set; }
        public string RapportLabel
        {
            get
            { return this.MeterType[0].ToString().ToUpper() + this.MeterType.Substring(1); }
        }

        [DisplayName("Meter identificatie")]
        public string MeterIdentificatie { get; set; }

        [DisplayName("Meter start datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public System.DateTime Startdate { get; set; }

        [DisplayName("Meter eind datum")]
        public Nullable<System.DateTime> Enddate { get; set; }

        [DisplayName("Meenemen in rapportage")]
        public bool MeterRapportMaken { get; set; } 

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
                if (this.MeterType == "gas")
                {
                    return "m3";
                }
                else
                {
                    return "KwH";
                }
            }
        }     

        // Lijsten met ruwe meter data
        public List<RawGasDagMeting> ReferenceGasDagMetingLijst = new List<RawGasDagMeting>();
        public List<RawStroomDagMeting> ReferenceStroomMetingen = new List<RawStroomDagMeting>();
        public List<RawGasDagMeting> UsageGasDagMetingLijst = new List<RawGasDagMeting>();
        public List<RawStroomDagMeting> UsageStroomMetingen = new List<RawStroomDagMeting>();
        public List<RawGasDagMeting> MaandGasDagMetingLijst = new List<RawGasDagMeting>();
        public List<RawStroomDagMeting> MaandStroomMetingen = new List<RawStroomDagMeting>();

        // Lijsten met per uur geconsolideerde data 
        public List<DagVerbruik> DagVerbruiken = new List<DagVerbruik>();
        public List<DagVerbruik> ReferenceDagVerbruiken = new List<DagVerbruik>();
        
        // Lijsten met statistieken 
        public DagRapportStatistieken DagRapportStatistieken = new DagRapportStatistieken();

        // Lijsten met maandverbruik per jaar
        public List<MaandVerbruikLijst> MaandVerbruiken = new List<MaandVerbruikLijst>() ;
        public MaandRapportStatistieken MaandRapportStatistieken = new MaandRapportStatistieken();
        private readonly string[] MaandArray = { "Jan", "Feb", "Mrt", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec" };
        public string LastMonthDate {  get; set; }
        public decimal LastMonthForecast { get; set; }  
        public int LastMonthLevel { get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null);
                int maand = lm.Month;
                return this.MaandRapportStatistieken.GetLevel(this.LastMonthForecast, this.MaandRapportStatistieken.MaandGegevens[maand-1].StatistiekWaardenM);
            } 
        }
        public string LastMonthName { get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null).AddDays(-1);
                int maand = lm.Month;
                return this.MaandArray[maand - 1];
            } 
        }
        public int LastMonthNumber
        {
            get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null).AddDays(-1);
                int maand = lm.Month;
                return maand;
            }
        }
        public decimal LastYearForecast { get; set; }
        public decimal LastYearLevel { get
            {
                return this.MaandRapportStatistieken.GetLevel(this.LastYearForecast, this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM);
            }
        }
        public string LastYearName
        {
            get
            {
                DateTime lm = DateTime.ParseExact(this.LastMonthDate, "dd-MM-yyyy", null).AddDays(-1);
                int year = lm.Year;
                return year.ToString("D2");
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
            public int Level00 { get; set; }
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

        // Hierna algemene methods
        public async Task<string> HaalEenDagRuwVerbruik(string datestring, string apikey, string listtype)
        {
            string url = "https://app.slimmemeterportal.nl/userapi/v1/connections/" + this.MeterIdentificatie.Trim() + "/usage/" + datestring;

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

                switch (this.MeterType)
                {
                    case "gas":
                        RawGasDagMeting gasVerbruik = new RawGasDagMeting
                        {
                            MeterIdentificatie = this.MeterIdentificatie,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            GasTimeSlotMetingLijst = JsonConvert.DeserializeObject<GasTimeSlotMetingLijst>(responseString)
                        };
                        switch (listtype)
                        {
                            case "Lastdate":
                                if (gasVerbruik.GasTimeSlotMetingLijst.usages.Count > 0) { result = "Last"; }
                                ;
                                break;
                            case "Usage":
                                this.UsageGasDagMetingLijst.Add(gasVerbruik);
                                break;
                            case "Reference":
                                this.ReferenceGasDagMetingLijst.Add(gasVerbruik);
                                break;
                            case "Month":
                                this.MaandGasDagMetingLijst.Add(gasVerbruik);
                                break;
                        }
                        break;

                    case "elektriciteit":
                        RawStroomDagMeting stroomVerbruik = new RawStroomDagMeting
                        {
                            MeterIdentificatie = this.MeterIdentificatie,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            StroomTimeSlotMetingLijst = JsonConvert.DeserializeObject<StroomTimeSlotMetingLijst>(responseString)
                        };
                        switch (listtype)
                        {
                            case "Lastdate":
                                if (stroomVerbruik.StroomTimeSlotMetingLijst.usages.Count > 0) { result = "Last"; }
                                ;
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
                        throw new Exception("Unknown device type " + this.MeterType);

                }

            }
            return result;


        }
        public void GasConsolidatie(string listtype)
        {
            List<RawGasDagMeting> listIN;
            if (listtype == "Usage")
            {
                listIN = this.UsageGasDagMetingLijst;  
            }
            else
            {
                listIN = this.ReferenceGasDagMetingLijst;
            }     

            foreach (RawGasDagMeting gasverbruik in listIN)
            {
                DagVerbruik dagVerbruik = new DagVerbruik
                {
                    VerbruiksDatum = gasverbruik.VerbruiksDatum,
                    VerbruiksType = "Gas",
                    MeterIdentificatie = gasverbruik.MeterIdentificatie
                };

                int verbruiksdag = gasverbruik.VerbruiksDatum.Day;
                int uurnummer = 0;
                int aantalmetingen = 0;
                decimal uurverbruik = 0;
                decimal endreading = 0;
                string uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                DateTime currentdate = DateTime.MinValue;
                string utc = ""; 

                foreach (GasTimeSlotMeting gastimeslotmeting in gasverbruik.GasTimeSlotMetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(gastimeslotmeting.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    utc = gastimeslotmeting.time.Substring(20,3);
                    int currenthour = currentdate.Hour;
                    int currentday = currentdate.Day;
                    if (currentday != verbruiksdag)
                    {
                        currenthour = 24;
                    }
                    int d = Int32.Parse(gastimeslotmeting.delivery.Replace(",", "").Replace(".", ""));
                    decimal ddec = d;
                    decimal delivery = ddec / 100;
                    uurverbruik += delivery;
                    aantalmetingen++;                   

                    if (currenthour == uurnummer)
                    {
                        // Just keep on filling the hour usage entry while in this hour
                        continue;
                    }
                    // Get delivery reading to report End Reading of past hour
                    int r = Int32.Parse(gastimeslotmeting.delivery_reading.Replace(",", "").Replace(".", ""));
                    decimal rdec = r;
                    endreading = rdec / 100;

                    for (int i = uurnummer; i < currenthour; i++)
                    {
                        if ((uurnummer + 1) == currenthour)
                        // Next hour starts here, so output current hour
                        {
                            UurVerbruik UurVerbruik = new UurVerbruik()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                Waarde = uurverbruik,
                                AantalMetingen = aantalmetingen,
                                EndReading = endreading,
                            };
                            dagVerbruik.UurVerbruiken.Add(UurVerbruik);

                            uurnummer++;
                            aantalmetingen = 0;
                            uurverbruik = 0;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                        else
                        {
                            // Apparantly we are missing an hour, so just output a zero hour usage
                            UurVerbruik UurVerbruik = new UurVerbruik()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                Waarde = 0,
                                AantalMetingen = 0,
                                EndReading = endreading
                            };
                            dagVerbruik.UurVerbruiken.Add(UurVerbruik);
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
                        UurVerbruik UurVerbruik = new UurVerbruik()
                        {
                            UurLabel = uurlabel,
                            UTC = utc,
                            VerbruiksTijdstip = currentdate,
                            UurNummer = uurnummer,
                            Waarde = 0,
                            AantalMetingen = 0, 
                            EndReading = endreading
                        };
                        dagVerbruik.UurVerbruiken.Add(UurVerbruik);
                        uurnummer++;

                    }

                }
                if (listtype == "Usage")
                {
                    this.DagVerbruiken.Add(dagVerbruik);
                }
                else
                {
                    this.ReferenceDagVerbruiken.Add(dagVerbruik);
                }                
            }

            return;
        }
        public void StroomConsolidatie(string listtype)
        {
            List<RawStroomDagMeting> listIN;
            if (listtype == "Usage")
            {
                listIN = this.UsageStroomMetingen;
            }
            else
            {
                listIN = this.ReferenceStroomMetingen;
            }

            foreach (RawStroomDagMeting stroomverbruik in listIN)
            {
                DagVerbruik dagVerbruik = new DagVerbruik
                {
                    VerbruiksDatum = stroomverbruik.VerbruiksDatum,
                    VerbruiksType = "Stroom",
                    MeterIdentificatie = stroomverbruik.MeterIdentificatie
                };

                int verbruiksdag = stroomverbruik.VerbruiksDatum.Day;
                int uurnummer = 0;
                int aantalmetingen = 0;
                decimal uurverbruik = 0;
                decimal endreading = 0;
                string uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                DateTime currentdate = DateTime.MinValue;
                string utc = "";

                foreach (StroomTimeSlotMeting stroomtimeslotmeting in stroomverbruik.StroomTimeSlotMetingLijst.usages)
                {
                    currentdate = DateTime.ParseExact(stroomtimeslotmeting.time.Substring(0, 19), "dd-MM-yyyy HH:mm:ss", null);
                    utc = stroomtimeslotmeting.time.Substring(20, 3);
                    int currenthour = currentdate.Hour;
                    int currentday = currentdate.Day;
                    int d1;
                    int d2;
                    if (currentday != verbruiksdag)
                    {
                        currenthour = 24;
                    }
                    if (string.IsNullOrEmpty(stroomtimeslotmeting.delivery_high))
                    {
                        d1 = 0;
                    }
                    else
                    {
                        d1 = Int32.Parse(stroomtimeslotmeting.delivery_high.Replace(",", "").Replace(".", ""));
                    }
                    if (string.IsNullOrEmpty(stroomtimeslotmeting.delivery_low))
                    {
                        d2 = 0;
                    }
                    else
                    {
                        d2 = Int32.Parse(stroomtimeslotmeting.delivery_low.Replace(",", "").Replace(".", ""));
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
                    // Get delivery reading to report End Reading of past hour
                    int r = Int32.Parse(stroomtimeslotmeting.delivery_reading_combined.Replace(",", "").Replace(".", ""));
                    decimal rdec = r;
                    endreading = rdec / 100;

                    for (int i = uurnummer; i < currenthour; i++)
                    {
                        if ((uurnummer + 1) == currenthour)
                        {
                            UurVerbruik UurVerbruik = new UurVerbruik()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                Waarde = uurverbruik,
                                AantalMetingen = aantalmetingen,
                                EndReading = endreading,
                            };
                            dagVerbruik.UurVerbruiken.Add(UurVerbruik);

                            uurnummer++;
                            aantalmetingen = 0;
                            uurverbruik = 0;
                            uurlabel = uurnummer.ToString("D2") + "-" + (uurnummer + 1).ToString("D2") + " Uur";
                        }
                        else
                        {
                            UurVerbruik UurVerbruik = new UurVerbruik()
                            {
                                UurLabel = uurlabel,
                                UTC = utc,
                                VerbruiksTijdstip = currentdate,
                                UurNummer = uurnummer,
                                Waarde = 0,
                                AantalMetingen = 0,
                                EndReading = endreading,
                            };
                            dagVerbruik.UurVerbruiken.Add(UurVerbruik);
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
                        UurVerbruik UurVerbruik = new UurVerbruik()
                        {
                            UurLabel = uurlabel,
                            UTC = utc,
                            VerbruiksTijdstip = currentdate,
                            UurNummer = uurnummer,
                            Waarde = 0,
                            AantalMetingen = 0,
                            EndReading = endreading,
                        };
                        dagVerbruik.UurVerbruiken.Add(UurVerbruik);
                        uurnummer++;

                    }

                }
                if (listtype == "Usage")
                {
                    this.DagVerbruiken.Add(dagVerbruik);
                }
                else
                {
                    this.ReferenceDagVerbruiken.Add(dagVerbruik);
                }
            }

            return;
        }

        public void BerekenDagRapportStatistieken()
        {
            
            for (int i = 0; i < 24; i++)
            {
                UurVerbruiksWaarden t = new UurVerbruiksWaarden();
                this.DagRapportStatistieken.UurVerbruiksWaardenLijst.Add(t);
            }

            // First fill waarnemingenlijst
            foreach (DagVerbruik dagverbruik in this.ReferenceDagVerbruiken)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (dagverbruik.UurVerbruiken[i].UurNummer != i)
                    {
                        string u = i.ToString("D2");
                        string d = dagverbruik.VerbruiksDatum.ToString("dd-MM-yyyy");
                        throw new Exception("Uurnummer " + u + "niet gevonden bij datum " + d);
                    }

                    this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].UurLabel = dagverbruik.UurVerbruiken[i].UurLabel;
                    int N = dagverbruik.UurVerbruiken[i].AantalMetingen;
                    if (N != 0)
                    {
                        this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].AantalWaarnemingen++;
                    }
                    this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].Uurwaarden.Add(dagverbruik.UurVerbruiken[i].Waarde);
                }

                this.DagRapportStatistieken.DagVerbruiksWaarden.Dagwaarden.Add(dagverbruik.TotaalperDag);
                this.DagRapportStatistieken.DagVerbruiksWaarden.Datums.Add(dagverbruik.VerbruiksDatum);

            }

            // Bereken percentielen per uur en per dag
            for (int i = 0; i < 24; i++)
            {
                this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden = GetNumbers(this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].Uurwaarden);
            }
            this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden = GetNumbers(this.DagRapportStatistieken.DagVerbruiksWaarden.Dagwaarden);

            return;
        }

        public StatistiekWaarden GetNumbers(List<decimal> getallenlijst)
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

            StatistiekWaarden StatistiekWaarden = new StatistiekWaarden
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

            return StatistiekWaarden;
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
                Dag1 = this.DagVerbruiken[0].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag2 = this.DagVerbruiken[1].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag3 = this.DagVerbruiken[2].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag4 = this.DagVerbruiken[3].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag5 = this.DagVerbruiken[4].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag6 = this.DagVerbruiken[5].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag7 = this.DagVerbruiken[6].VerbruiksDatum.ToString("yyyy-MM-dd")
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
                Dag1 = this.DagVerbruiken[0].UTC,
                Dag2 = this.DagVerbruiken[1].UTC,
                Dag3 = this.DagVerbruiken[2].UTC,
                Dag4 = this.DagVerbruiken[3].UTC,
                Dag5 = this.DagVerbruiken[4].UTC,
                Dag6 = this.DagVerbruiken[5].UTC,
                Dag7 = this.DagVerbruiken[6].UTC
            };
            this.DagRapport.Add(lineu);

            for (int i = 0; i < 24; i++)
            {
                RPT_line lined = new RPT_line
                {
                    N = this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].AantalWaarnemingen.ToString("D2"),
                    Min = this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden.Min.ToString("N2"),
                    Mean = this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden.Mean.ToString("N2"),
                    Max = this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden.Max.ToString("N2"),
                    Label = this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].UurLabel,
                    Eenheid = this.Eenheid,
                    Dag1 = this.DagVerbruiken[0].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[0].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag2 = this.DagVerbruiken[1].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[1].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag3 = this.DagVerbruiken[2].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[2].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag4 = this.DagVerbruiken[3].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[3].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag5 = this.DagVerbruiken[4].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[4].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag6 = this.DagVerbruiken[5].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[5].UurVerbruiken[i].Waarde.ToString("N2"),
                    Dag7 = this.DagVerbruiken[6].UurVerbruiken[i].AantalMetingen == 0 ? "n/a" : this.DagVerbruiken[6].UurVerbruiken[i].Waarde.ToString("N2"),
                    Level1 = this.DagVerbruiken[0].UurVerbruiken[i].AantalMetingen == 0 ? 9 :
                            this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[0].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level2 = this.DagVerbruiken[0].UurVerbruiken[i].AantalMetingen == 0 ? 9 :
                            this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[1].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level3 = this.DagVerbruiken[2].UurVerbruiken[i].AantalMetingen == 0 ? 9 : this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[2].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level4 = this.DagVerbruiken[3].UurVerbruiken[i].AantalMetingen == 0 ? 9 : this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[3].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level5 = this.DagVerbruiken[4].UurVerbruiken[i].AantalMetingen == 0 ? 9 : this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[4].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level6 = this.DagVerbruiken[5].UurVerbruiken[i].AantalMetingen == 0 ? 9 : this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[5].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden),
                    Level7 = this.DagVerbruiken[6].UurVerbruiken[i].AantalMetingen == 0 ? 9 : this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[6].UurVerbruiken[i].Waarde, this.DagRapportStatistieken.UurVerbruiksWaardenLijst[i].StatistiekWaarden)
                };
                this.DagRapport.Add(lined);

            }
            RPT_line linex = new RPT_line
            {
                N = this.DagRapportStatistieken.DagVerbruiksWaarden.AantalWaarnemingen.ToString("D2"),
                Min = this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden.Min.ToString("N2"),
                Mean = this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden.Mean.ToString("N2"),
                Max = this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden.Max.ToString("N2"),
                Label = "Dag Totaal (SumUp)",
                Eenheid = this.Eenheid,
                Dag1 = this.DagVerbruiken[0].TotaalperDag.ToString("N2"),
                Dag2 = this.DagVerbruiken[1].TotaalperDag.ToString("N2"),
                Dag3 = this.DagVerbruiken[2].TotaalperDag.ToString("N2"),
                Dag4 = this.DagVerbruiken[3].TotaalperDag.ToString("N2"),
                Dag5 = this.DagVerbruiken[4].TotaalperDag.ToString("N2"),
                Dag6 = this.DagVerbruiken[5].TotaalperDag.ToString("N2"),
                Dag7 = this.DagVerbruiken[6].TotaalperDag.ToString("N2"),
                Level1 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[0].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level2 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[1].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level3 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[2].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level4 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[3].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level5 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[4].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level6 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[5].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden),
                Level7 = this.DagRapportStatistieken.GetLevel(this.DagVerbruiken[6].TotaalperDag, this.DagRapportStatistieken.DagVerbruiksWaarden.StatistiekWaarden)                
            };
            this.DagRapport.Add(linex);

            //* add totals based on diff calculation
            RPT_line linex2 = new RPT_line
            {                
                N = "",
                Min = "",
                Mean = "",
                Max = "",
                Label = "Dag Totaal (Diff)",
                Eenheid = this.Eenheid,
                Dag1 = this.DagVerbruiken[0].DiffperDag.ToString("N2"),
                Dag2 = this.DagVerbruiken[1].DiffperDag.ToString("N2"),
                Dag3 = this.DagVerbruiken[2].DiffperDag.ToString("N2"),
                Dag4 = this.DagVerbruiken[3].DiffperDag.ToString("N2"),
                Dag5 = this.DagVerbruiken[4].DiffperDag.ToString("N2"),
                Dag6 = this.DagVerbruiken[5].DiffperDag.ToString("N2"),
                Dag7 = this.DagVerbruiken[6].DiffperDag.ToString("N2"),
                Level1 = 9,
                Level2 = 9,
                Level3 = 9,
                Level4 = 9,
                Level5 = 9,
                Level6 = 9,
                Level7 = 9
            };
            this.DagRapport.Add(linex2);

            return;
        }
        // Hieronder bgeginnen de maand rapport routines
        public async Task<string> GetSMPmonth(int year, string apikey)
        {
            string datestring;
            
            for (int mo = 1; mo <= 12; mo++)
            {
                datestring = "01-" + mo.ToString("D2") + "-" + year.ToString("D4");

                Task<string> longRunningTask = this.HaalEenDagRuwVerbruik(datestring, apikey, "Month");
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
                    Task<string> longRunningTask2 = this.HaalEenDagRuwVerbruik(datestring, apikey, "Month");
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
                        
            int startyear = this.MaandGasDagMetingLijst[0].VerbruiksDatum.Year;
            int endyear = this.RapportageDatum.Year;            

            for (int y = startyear; y <= endyear; y++)
            {
                MaandVerbruikLijst maandverbruik = new MaandVerbruikLijst();
                bool newyear = true;
                foreach (RawGasDagMeting GasDagMeting in this.MaandGasDagMetingLijst)
                {
                    if (GasDagMeting.VerbruiksDatum.Year < y) { continue; }

                    maandnr = GasDagMeting.VerbruiksDatum.Month;

                    if (newyear)
                    {
                        if (maandnr != 1)
                        {
                            throw new Exception("Maandoverzicht kan niet starten met een jaar dat start met maand " + maandnr.ToString());
                        }
                        else
                        {                            
                            // eerste maand van het jaar
                            maandverbruik.MeterIdentificatie = GasDagMeting.MeterIdentificatie;
                            maandverbruik.Jaar = GasDagMeting.VerbruiksDatum.Year;
                            maandverbruik.VerbruiksType = "gas";

                            string strt = GasDagMeting.GasTimeSlotMetingLijst.usages[0].delivery_reading; // meterstand
                            string deliv = GasDagMeting.GasTimeSlotMetingLijst.usages[0].delivery;

                            int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                            decimal dstart = istart;

                            int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                            decimal ddeliv = ideliv;

                            // de eerste entry van de dag bevat de meterstand aan het einde van het eerste uur. 
                            // de delivery eraf trekken geeft de stand om 00:00 uur
                            startreading = (dstart - ddeliv) / 100; // start stand van het jaar 

                            currentyear = GasDagMeting.VerbruiksDatum.Year;

                            newyear = false;

                            // Get meterstanden

                            meterstandtotaal = startreading;

                        }
                    }
                    else
                    {
                        if (GasDagMeting.GasTimeSlotMetingLijst.usages.Count == 0) 
                        {
                            throw new Exception("Lege meting: jaar = " + currentyear.ToString() + " maand = " + maandnr.ToString());
                        }

                        string strt = GasDagMeting.GasTimeSlotMetingLijst.usages[0].delivery_reading;
                        string deliv = GasDagMeting.GasTimeSlotMetingLijst.usages[0].delivery;

                        int istart = Int32.Parse(strt.Replace(",", "").Replace(".", ""));
                        decimal dstart = istart;

                        int ideliv = Int32.Parse(deliv.Replace(",", "").Replace(".", ""));
                        decimal ddeliv = ideliv;

                        endreading = (dstart - ddeliv) / 100;

                        decimal monthusage = endreading - startreading;
                        
                        if (GasDagMeting.VerbruiksDatum.Day == 1)
                        {
                            maandnr = GasDagMeting.VerbruiksDatum.Month - 1;
                            if (maandnr == 0) { maandnr = 12; }

                        }
                        else
                        {
                            maandnr = GasDagMeting.VerbruiksDatum.Month;
                            
                        }     
                        maandlabel = this.MaandArray[maandnr - 1];
                        MaandVerbruik MaandVerbruik = new MaandVerbruik
                        {
                            MaandLabel = maandlabel,
                            MaandNummer = maandnr,
                            Waarde = monthusage,
                            MeterstandTotaal = meterstandtotaal,
                            MeterstandLaagTarief = 0,
                            MeterstandNormaalTarief = meterstandtotaal
                        };
                        maandverbruik.MaandVerbruiken.Add(MaandVerbruik);

                        startreading = endreading;

                        meterstandtotaal = startreading;

                        if (currentyear != GasDagMeting.VerbruiksDatum.Year)
                        {
                            this.MaandVerbruiken.Add(maandverbruik);
                            break;
                        }

                        
                    }
                }
                if (currentyear == endyear)
                {
                    // Add last (incomplete) year
                    int q = maandverbruik.MaandVerbruiken.Count;
                    int maxdays = DateTime.DaysInMonth(currentyear, maandverbruik.MaandVerbruiken[q - 1].MaandNummer);
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
                    
                    this.LastMonthForecast = maandverbruik.MaandVerbruiken[q - 1].Waarde * maxdays / nrofdays;
                    
                    // Add dummy entries for months that lay in the future
                    for (int j = q; j <= 12; j++)
                    {
                        MaandVerbruik dummy = new MaandVerbruik
                        {
                            MaandLabel = this.MaandArray[j - 1],
                            MaandNummer = j,
                            Waarde = 0
                        };
                        maandverbruik.MaandVerbruiken.Add(dummy);
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
                MaandVerbruikLijst maandverbruik = new MaandVerbruikLijst();
                bool newyear = true;
                foreach (RawStroomDagMeting StroomMetingen in this.MaandStroomMetingen)
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
                            maandverbruik.MeterIdentificatie = StroomMetingen.MeterIdentificatie;
                            maandverbruik.Jaar = StroomMetingen.VerbruiksDatum.Year;
                            maandverbruik.VerbruiksType = "elektriciteit";

                            string strt = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_reading_combined; // meterstand
                            string deliv = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_high;
                            if (string.IsNullOrEmpty(deliv)) {
                                deliv = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_low;
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
                        if (StroomMetingen.StroomTimeSlotMetingLijst.usages.Count == 0)
                        {
                            throw new Exception("Lege meting: jaar = " + currentyear.ToString() + " maand = " + maandnr.ToString());
                        }
                        
                        string strt = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_reading_combined;
                        string deliv = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_high;
                        if (string.IsNullOrEmpty(deliv)) {
                            deliv = StroomMetingen.StroomTimeSlotMetingLijst.usages[0].delivery_low;
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
                        MaandVerbruik MaandVerbruik = new MaandVerbruik
                        {
                            MaandLabel = maandlabel,
                            MaandNummer = maandnr,
                            Waarde = monthusage,
                            MeterstandTotaal = meterstandtotaal,
                            MeterstandNormaalTarief = meterstandtotaal,
                            MeterstandLaagTarief = 0
                        };

                        maandverbruik.MaandVerbruiken.Add(MaandVerbruik);
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
                    int q = maandverbruik.MaandVerbruiken.Count;
                    int maxdays = DateTime.DaysInMonth(currentyear, maandverbruik.MaandVerbruiken[q - 1].MaandNummer);
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

                    this.LastMonthForecast = maandverbruik.MaandVerbruiken[q - 1].Waarde * maxdays / nrofdays;

                    // Add dummy entries for months that lay in the future
                    for (int j = q; j <= 12; j++)
                    {
                        MaandVerbruik dummy = new MaandVerbruik
                        {
                            MaandLabel = this.MaandArray[j - 1],
                            MaandNummer = j,
                            Waarde = 0
                        };
                        maandverbruik.MaandVerbruiken.Add(dummy);
                    }
                    this.MaandVerbruiken.Add(maandverbruik);
                }
            }

        }

        public void MonthStats ()
        {
            for (int i = 0; i < 13; i++)
            {
               MaandGegeven MaandGegeven = new MaandGegeven
               {
                    Maandnr = i + 1,                    
                    AantalWaarnemingen = 0,
                };
                if (i < 12) {
                    MaandGegeven.MaandLabel = this.MaandArray[i];
                }
                else
                {
                    MaandGegeven.MaandLabel = "jaartotaal"; 
                }
                MaandGegeven.StatistiekWaardenM.Max = int.MinValue;
                MaandGegeven.StatistiekWaardenM.Min = int.MaxValue;
                this.MaandRapportStatistieken.MaandGegevens.Add(MaandGegeven);
            }

            // get statistics for each month
            for (int i = 0; i < 12; i++)
            {
                decimal sumup = 0;
                
                foreach (MaandVerbruikLijst maandverbruik in this.MaandVerbruiken)
                {
                    if (maandverbruik.Jaar == this.RapportageDatum.Year) 
                    {                        
                        break; 
                    }
                    if ((maandverbruik.MaandVerbruiken[i].MaandNummer != i + 1) && (maandverbruik.Jaar != this.RapportageDatum.Year))
                    {
                        throw new Exception("Maandnummer matcht niet in MONTHSTATS");
                    }
                    this.MaandRapportStatistieken.MaandGegevens[i].AantalWaarnemingen++;
                    decimal c = maandverbruik.MaandVerbruiken[i].Waarde;
                    sumup += c;
                    if (c > this.MaandRapportStatistieken.MaandGegevens[i].StatistiekWaardenM.Max)
                    {
                        this.MaandRapportStatistieken.MaandGegevens[i].StatistiekWaardenM.Max = c;
                    }
                    if (c < this.MaandRapportStatistieken.MaandGegevens[i].StatistiekWaardenM.Min)
                    {
                        this.MaandRapportStatistieken.MaandGegevens[i].StatistiekWaardenM.Min = c;
                    }
                }
                this.MaandRapportStatistieken.MaandGegevens[i].StatistiekWaardenM.Mean = sumup / this.MaandRapportStatistieken.MaandGegevens[i].AantalWaarnemingen;                
            }

            
            // Create yeartotal statitics in an extra entry 
            decimal Ymin = decimal.MaxValue;
            decimal Ymax = decimal.MinValue;
            decimal Ysum = 0;
            int N = 0;
            
            foreach (MaandVerbruikLijst m in this.MaandVerbruiken)
            {
                if (m.Jaar == this.RapportageDatum.Year)
                {
                    break;
                }
                N++; // Aantal waarnemingen
                // Referentie Jaarcijfers
                
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
            this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Max = Ymax;
            this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Min = Ymin;
            this.MaandRapportStatistieken.MaandGegevens[12].AantalWaarnemingen = N;
            this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Mean = Ysum / this.MaandRapportStatistieken.MaandGegevens[12].AantalWaarnemingen;

            // Tot slot: Bepaal forecast voor het lopende jaar
            // 1: bepaal gemiddeld totaal per jaar
            decimal meantotal = 0;
            foreach (MaandGegeven mg in this.MaandRapportStatistieken.MaandGegevens)
            {
                if (mg.MaandLabel == "jaartotaal" )
                {
                    break;
                }
                meantotal += mg.StatistiekWaardenM.Mean;
            }
            // 2: bepaal gemiddeld totaal t/m de laatste rapportmaand
            decimal meantonow = 0;                        
            foreach (MaandGegeven mg in this.MaandRapportStatistieken.MaandGegevens)
            {
                if (mg.MaandLabel == "jaartotaal")
                {
                    break;
                }
                meantonow += mg.StatistiekWaardenM.Mean;
                if (mg.Maandnr == this.LastMonthNumber)
                {
                    break;
                }
            }
            // 3: bepaal werkelijk totaal t/m de laatste rapportmaand
            decimal realtonow = 0;
            int cnt = this.MaandVerbruiken.Count - 1;           
            foreach (MaandVerbruik mc in this.MaandVerbruiken[cnt].MaandVerbruiken) {                
                if (mc.MaandNummer == this.LastMonthNumber)
                {
                    realtonow += this.LastMonthForecast;
                    break;
                }
                else
                {
                    realtonow += mc.Waarde;
                }                
            }
            // 4: verhouding tussen werkelijk gebruik en gemiddeld gebruike bepaalt de forecast
            this.LastYearForecast = meantotal * realtonow / meantonow;

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
                Totaal = this.MaandRapportStatistieken.MaandGegevens[12].AantalWaarnemingen.ToString("D2"),
                Maand01 = this.MaandRapportStatistieken.MaandGegevens[0].AantalWaarnemingen.ToString("D2"),
                Maand02 = this.MaandRapportStatistieken.MaandGegevens[1].AantalWaarnemingen.ToString("D2"),
                Maand03 = this.MaandRapportStatistieken.MaandGegevens[2].AantalWaarnemingen.ToString("D2"),
                Maand04 = this.MaandRapportStatistieken.MaandGegevens[3].AantalWaarnemingen.ToString("D2"),
                Maand05 = this.MaandRapportStatistieken.MaandGegevens[4].AantalWaarnemingen.ToString("D2"),
                Maand06 = this.MaandRapportStatistieken.MaandGegevens[5].AantalWaarnemingen.ToString("D2"),
                Maand07 = this.MaandRapportStatistieken.MaandGegevens[6].AantalWaarnemingen.ToString("D2"),
                Maand08 = this.MaandRapportStatistieken.MaandGegevens[7].AantalWaarnemingen.ToString("D2"),
                Maand09 = this.MaandRapportStatistieken.MaandGegevens[8].AantalWaarnemingen.ToString("D2"),
                Maand10 = this.MaandRapportStatistieken.MaandGegevens[9].AantalWaarnemingen.ToString("D2"),
                Maand11 = this.MaandRapportStatistieken.MaandGegevens[10].AantalWaarnemingen.ToString("D2"),
                Maand12 = this.MaandRapportStatistieken.MaandGegevens[11].AantalWaarnemingen.ToString("D2"),

                Level00 = 9,
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
                Totaal = this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Min.ToString("N2"),
                Maand01 = this.MaandRapportStatistieken.MaandGegevens[0].StatistiekWaardenM.Min.ToString("N2"),
                Maand02 = this.MaandRapportStatistieken.MaandGegevens[1].StatistiekWaardenM.Min.ToString("N2"),
                Maand03 = this.MaandRapportStatistieken.MaandGegevens[2].StatistiekWaardenM.Min.ToString("N2"),
                Maand04 = this.MaandRapportStatistieken.MaandGegevens[3].StatistiekWaardenM.Min.ToString("N2"),
                Maand05 = this.MaandRapportStatistieken.MaandGegevens[4].StatistiekWaardenM.Min.ToString("N2"),
                Maand06 = this.MaandRapportStatistieken.MaandGegevens[5].StatistiekWaardenM.Min.ToString("N2"),
                Maand07 = this.MaandRapportStatistieken.MaandGegevens[6].StatistiekWaardenM.Min.ToString("N2"),
                Maand08 = this.MaandRapportStatistieken.MaandGegevens[7].StatistiekWaardenM.Min.ToString("N2"),
                Maand09 = this.MaandRapportStatistieken.MaandGegevens[8].StatistiekWaardenM.Min.ToString("N2"),
                Maand10 = this.MaandRapportStatistieken.MaandGegevens[9].StatistiekWaardenM.Min.ToString("N2"),
                Maand11 = this.MaandRapportStatistieken.MaandGegevens[10].StatistiekWaardenM.Min.ToString("N2"),
                Maand12 = this.MaandRapportStatistieken.MaandGegevens[11].StatistiekWaardenM.Min.ToString("N2"),

                Level00 = 9,
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
                Totaal = this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Mean.ToString("N2"),
                Maand01 = this.MaandRapportStatistieken.MaandGegevens[0].StatistiekWaardenM.Mean.ToString("N2"),
                Maand02 = this.MaandRapportStatistieken.MaandGegevens[1].StatistiekWaardenM.Mean.ToString("N2"),
                Maand03 = this.MaandRapportStatistieken.MaandGegevens[2].StatistiekWaardenM.Mean.ToString("N2"),
                Maand04 = this.MaandRapportStatistieken.MaandGegevens[3].StatistiekWaardenM.Mean.ToString("N2"),
                Maand05 = this.MaandRapportStatistieken.MaandGegevens[4].StatistiekWaardenM.Mean.ToString("N2"),
                Maand06 = this.MaandRapportStatistieken.MaandGegevens[5].StatistiekWaardenM.Mean.ToString("N2"),
                Maand07 = this.MaandRapportStatistieken.MaandGegevens[6].StatistiekWaardenM.Mean.ToString("N2"),
                Maand08 = this.MaandRapportStatistieken.MaandGegevens[7].StatistiekWaardenM.Mean.ToString("N2"),
                Maand09 = this.MaandRapportStatistieken.MaandGegevens[8].StatistiekWaardenM.Mean.ToString("N2"),
                Maand10 = this.MaandRapportStatistieken.MaandGegevens[9].StatistiekWaardenM.Mean.ToString("N2"),
                Maand11 = this.MaandRapportStatistieken.MaandGegevens[10].StatistiekWaardenM.Mean.ToString("N2"),
                Maand12 = this.MaandRapportStatistieken.MaandGegevens[11].StatistiekWaardenM.Mean.ToString("N2"),

                Level00 = 9,
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
                Totaal = this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM.Max.ToString("N2"),
                Maand01 = this.MaandRapportStatistieken.MaandGegevens[0].StatistiekWaardenM.Max.ToString("N2"),
                Maand02 = this.MaandRapportStatistieken.MaandGegevens[1].StatistiekWaardenM.Max.ToString("N2"),
                Maand03 = this.MaandRapportStatistieken.MaandGegevens[2].StatistiekWaardenM.Max.ToString("N2"),
                Maand04 = this.MaandRapportStatistieken.MaandGegevens[3].StatistiekWaardenM.Max.ToString("N2"),
                Maand05 = this.MaandRapportStatistieken.MaandGegevens[4].StatistiekWaardenM.Max.ToString("N2"),
                Maand06 = this.MaandRapportStatistieken.MaandGegevens[5].StatistiekWaardenM.Max.ToString("N2"),
                Maand07 = this.MaandRapportStatistieken.MaandGegevens[6].StatistiekWaardenM.Max.ToString("N2"),
                Maand08 = this.MaandRapportStatistieken.MaandGegevens[7].StatistiekWaardenM.Max.ToString("N2"),
                Maand09 = this.MaandRapportStatistieken.MaandGegevens[8].StatistiekWaardenM.Max.ToString("N2"),
                Maand10 = this.MaandRapportStatistieken.MaandGegevens[9].StatistiekWaardenM.Max.ToString("N2"),
                Maand11 = this.MaandRapportStatistieken.MaandGegevens[10].StatistiekWaardenM.Max.ToString("N2"),
                Maand12 = this.MaandRapportStatistieken.MaandGegevens[11].StatistiekWaardenM.Max.ToString("N2"),

                Level00 = 9,
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

            foreach (MaandVerbruikLijst m in this.MaandVerbruiken)
            {
                RPT_Mline lined = new RPT_Mline
                {
                    Jaar = m.Jaar.ToString("D2"),
                    Attribuut = "",
                    Eenheid = this.Eenheid,
                    Totaal = m.TotaalperJaar.ToString("N2"),
                    
                    Maand01 = (m.MaandVerbruiken[0].Waarde == 0) ? "n/a" : m.MaandVerbruiken[0].Waarde.ToString("N2"),
                    Maand02 = (m.MaandVerbruiken[1].Waarde == 0) ? "n/a" : m.MaandVerbruiken[1].Waarde.ToString("N2"),
                    Maand03 = (m.MaandVerbruiken[2].Waarde == 0) ? "n/a" : m.MaandVerbruiken[2].Waarde.ToString("N2"),
                    Maand04 = (m.MaandVerbruiken[3].Waarde == 0) ? "n/a" : m.MaandVerbruiken[3].Waarde.ToString("N2"),
                    Maand05 = (m.MaandVerbruiken[4].Waarde == 0) ? "n/a" : m.MaandVerbruiken[4].Waarde.ToString("N2"),
                    Maand06 = (m.MaandVerbruiken[5].Waarde == 0) ? "n/a" : m.MaandVerbruiken[5].Waarde.ToString("N2"),
                    Maand07 = (m.MaandVerbruiken[6].Waarde == 0) ? "n/a" : m.MaandVerbruiken[6].Waarde.ToString("N2"),
                    Maand08 = (m.MaandVerbruiken[7].Waarde == 0) ? "n/a" : m.MaandVerbruiken[7].Waarde.ToString("N2"),
                    Maand09 = (m.MaandVerbruiken[8].Waarde == 0) ? "n/a" : m.MaandVerbruiken[8].Waarde.ToString("N2"),
                    Maand10 = (m.MaandVerbruiken[9].Waarde == 0) ? "n/a" : m.MaandVerbruiken[9].Waarde.ToString("N2"),
                    Maand11 = (m.MaandVerbruiken[10].Waarde == 0) ? "n/a" : m.MaandVerbruiken[10].Waarde.ToString("N2"),
                    Maand12 = (m.MaandVerbruiken[11].Waarde == 0) ? "n/a" : m.MaandVerbruiken[11].Waarde.ToString("N2"),

                    Level00 = (m.TotaalperJaar == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.TotaalperJaar, this.MaandRapportStatistieken.MaandGegevens[12].StatistiekWaardenM),
                    Level01 = (m.MaandVerbruiken[0].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[0].Waarde, this.MaandRapportStatistieken.MaandGegevens[0].StatistiekWaardenM),
                    Level02 = (m.MaandVerbruiken[1].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[1].Waarde, this.MaandRapportStatistieken.MaandGegevens[1].StatistiekWaardenM),
                    Level03 = (m.MaandVerbruiken[2].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[2].Waarde, this.MaandRapportStatistieken.MaandGegevens[2].StatistiekWaardenM),
                    Level04 = (m.MaandVerbruiken[3].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[3].Waarde, this.MaandRapportStatistieken.MaandGegevens[3].StatistiekWaardenM),
                    Level05 = (m.MaandVerbruiken[4].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[4].Waarde, this.MaandRapportStatistieken.MaandGegevens[4].StatistiekWaardenM),
                    Level06 = (m.MaandVerbruiken[5].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[5].Waarde, this.MaandRapportStatistieken.MaandGegevens[5].StatistiekWaardenM),
                    Level07 = (m.MaandVerbruiken[6].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[6].Waarde, this.MaandRapportStatistieken.MaandGegevens[6].StatistiekWaardenM),
                    Level08 = (m.MaandVerbruiken[7].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[7].Waarde, this.MaandRapportStatistieken.MaandGegevens[7].StatistiekWaardenM),
                    Level09 = (m.MaandVerbruiken[8].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[8].Waarde, this.MaandRapportStatistieken.MaandGegevens[8].StatistiekWaardenM),
                    Level10 = (m.MaandVerbruiken[9].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[9].Waarde, this.MaandRapportStatistieken.MaandGegevens[9].StatistiekWaardenM),
                    Level11 = (m.MaandVerbruiken[10].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[10].Waarde, this.MaandRapportStatistieken.MaandGegevens[10].StatistiekWaardenM),
                    Level12 = (m.MaandVerbruiken[11].Waarde == 0) ? 9 : this.MaandRapportStatistieken.GetLevel(m.MaandVerbruiken[11].Waarde, this.MaandRapportStatistieken.MaandGegevens[11].StatistiekWaardenM),
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

            foreach (MaandVerbruikLijst m in this.MaandVerbruiken)
            {
                Meterstand Meterstand2 = new Meterstand
                {
                    Jaar = m.Jaar.ToString("D2"),
                    Eenheid = this.Eenheid,

                    Maand01 = m.MaandVerbruiken[0].MeterstandTotaal.ToString("N0"),
                    Maand02 = m.MaandVerbruiken[1].MeterstandTotaal.ToString("N0"),
                    Maand03 = m.MaandVerbruiken[2].MeterstandTotaal.ToString("N0"),
                    Maand04 = m.MaandVerbruiken[3].MeterstandTotaal.ToString("N0"),
                    Maand05 = m.MaandVerbruiken[4].MeterstandTotaal.ToString("N0"),
                    Maand06 = m.MaandVerbruiken[5].MeterstandTotaal.ToString("N0"),
                    Maand07 = m.MaandVerbruiken[6].MeterstandTotaal.ToString("N0"),
                    Maand08 = m.MaandVerbruiken[7].MeterstandTotaal.ToString("N0"),
                    Maand09 = m.MaandVerbruiken[8].MeterstandTotaal.ToString("N0"),
                    Maand10 = m.MaandVerbruiken[9].MeterstandTotaal.ToString("N0"),
                    Maand11 = m.MaandVerbruiken[10].MeterstandTotaal.ToString("N0"),
                    Maand12 = m.MaandVerbruiken[11].MeterstandTotaal.ToString("N0")
                };
                this.Meterstanden.Add(Meterstand2);
            }


                return;
           
        }
    }
}
