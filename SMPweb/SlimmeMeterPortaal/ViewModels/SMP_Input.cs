using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Schema;

namespace SlimmeMeterPortaal.ViewModels
{
    public class SMP_Input

    {
        public List<GasMetingen> GasVerbruikLijst = new List<GasMetingen>();
        public List<StroomMetingen> StroomVerbruikLijst = new List<StroomMetingen>();
        public List<DagVerbruik> GasUurLijst = new List<DagVerbruik>(); 
        public List<DagVerbruik> StroomUurLijst = new List<DagVerbruik>();

        public List<GasMetingen> GasReferenceLijst = new List<GasMetingen>();
        public List<StroomMetingen> StroomReferenceLijst = new List<StroomMetingen>();
        public List<DagVerbruik> GasUurReference = new List<DagVerbruik>();
        public List<DagVerbruik> StroomUurReference = new List<DagVerbruik>();

        public Stats GasStatistieken = new Stats();
        public Stats StroomStatistieken = new Stats();


        public async Task<string> GetUsage(SMP_Report smp_Report)
        {
            foreach (DeviceVM dvm in smp_Report.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                DateTime reportDate = smp_Report.Rapportagedatum;
                for (int i = -6; i <= 0; i++)
                {
                    DateTime entrydate = smp_Report.Rapportagedatum.AddDays(i);
                    string datestring = entrydate.ToString("dd-MM-yyyy");
                    Task<string> longRunningTask = this.GetSMPday(datestring, dvm, smp_Report.APIkey,this.GasVerbruikLijst,this.StroomVerbruikLijst);
                    string result = await longRunningTask;

                    if (result != "Ok")
                    {
                        throw new Exception("Function GetSMPday failed");

                    }
                }

            }
            return "Ok";
        }

        public async Task<string> GetSMPday(string datestring, DeviceVM deviceVM, string apikey, List<GasMetingen> gas, List<StroomMetingen> stroom)
        {
            string url = "https://app.slimmemeterportal.nl/userapi/v1/connections/" + deviceVM.DeviceID.Trim() + "/usage/" + datestring;

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

                switch (deviceVM.DeviceType)
                {
                    case "gas":
                        GasMetingen gasVerbruik = new GasMetingen
                        {
                            DeviceID = deviceVM.DeviceID,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),                           
                            MetingLijst = JsonConvert.DeserializeObject<GasDagMeting>(responseString)
                        };
                        gas.Add(gasVerbruik);
                        break;

                    case "elektriciteit":
                        StroomMetingen stroomVerbruik = new StroomMetingen
                        {
                            DeviceID = deviceVM.DeviceID,
                            VerbruiksDatum = DateTime.ParseExact(datestring, "dd-MM-yyyy", null),
                            MetingLijst = JsonConvert.DeserializeObject<StroomDagMeting>(responseString)
                        };
                        stroom.Add(stroomVerbruik);
                        break;

                    default:
                        throw new Exception("Unknown device type " + deviceVM.DeviceType);

                }

                return "Ok";
            }
            else
            {
                return "Nok";
            }


        }

        public async Task<string> GetReference(SMP_Report smp_Report)
        {
            foreach (DeviceVM dvm in smp_Report.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                for (int year = -1 * smp_Report.ReferentieJaren; year < 0; year++)
                {
                    DateTime refdate = smp_Report.Rapportagedatum.AddYears(year);
                    int two = 2;
                    int daymin = -1*(smp_Report.ReferentieDagen-1) / two;
                    int daymax = daymin + smp_Report.ReferentieDagen;
                    for (int day = daymin; day < daymax; day++)
                    {
                        DateTime entrydate = smp_Report.Rapportagedatum.AddYears(year).AddDays(day);
                        string datestring = entrydate.ToString("dd-MM-yyyy");
                        Task<string> longRunningTask = this.GetSMPday(datestring, dvm, smp_Report.APIkey, this.GasReferenceLijst, this.StroomReferenceLijst);
                        string result = await longRunningTask;
                        if (result != "Ok")
                        {
                            throw new Exception("Function GetSMPday failed");

                        }
                    }
                }     
            }
            return "Ok";
        }

        public void Consolidate()
        {
            this.GasUurLijst = this.GasConsolidatie(this.GasVerbruikLijst);
            this.GasUurReference = this.GasConsolidatie(this.GasReferenceLijst);
            this.StroomUurLijst = this.StroomConsolidatie(this.StroomVerbruikLijst);
            this.StroomUurReference = this.StroomConsolidatie(this.StroomReferenceLijst);
            return;

        }
        public List<DagVerbruik> GasConsolidatie(List<GasMetingen> listIN)
        {
            if (listIN == null) {  return null; };

            List<DagVerbruik> listOUT = new List<DagVerbruik>();
            
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
                string uurlabel = "Uur-" + (uurnummer+1).ToString("D2");
                DateTime currentdate = DateTime.MinValue;

                foreach (GasUsage gasusage in gasverbruik.MetingLijst.usages )
                {
                    currentdate = DateTime.ParseExact(gasusage.time.Substring(0,19), "dd-MM-yyyy HH:mm:ss", null);
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
                    if (currenthour == uurnummer) {
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
                            uurlabel = "Uur-" + (uurnummer+1).ToString("D2");
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
                            uurlabel = "Uur-" + (uurnummer+1).ToString("D2");
                        }     
                    }
                }
                if (uurnummer < 24)
                {
                    for (int i = uurnummer; i < 24; i++)
                    {
                        uurlabel = "Uur-" + (uurnummer + 1).ToString("D2");
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
                listOUT.Add(dagVerbruik);
            }

            return listOUT;
        }
        public List<DagVerbruik> StroomConsolidatie(List<StroomMetingen> listIN)
        {
            if (listIN == null) { return null; }
            List<DagVerbruik> listOUT = new List<DagVerbruik>();

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
                string uurlabel = "Uur-" + (uurnummer + 1).ToString("D2");
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
                            uurlabel = "Uur-" + (uurnummer + 1).ToString("D2");
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
                            uurlabel = "Uur-" + (uurnummer + 1).ToString("D2");
                        }
                    }
                }
                if (uurnummer < 24)
                {
                    for (int i = uurnummer; i < 24; i++)
                    {
                        uurlabel = "Uur-" + (uurnummer + 1).ToString("D2");
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
                listOUT.Add(dagVerbruik);
            }

            return listOUT;
        }
        public void Statistics()
        {
            this.GasStatistieken = this.GetStats(GasUurReference);
            this.StroomStatistieken = this.GetStats(StroomUurReference);
            return;
        }

        public Stats GetStats (List<DagVerbruik> dagverbruiklijst )
        {
            Stats stats = new Stats
            {
                VerbruiksType = dagverbruiklijst[0].VerbruiksType
            };


            for (int i  = 0; i < 24; i++)
            {
                UurStats t = new UurStats();
                stats.UurStatsList.Add(t);
            }

            // First fill waarnemingenlijst
            foreach (DagVerbruik dagverbruik in dagverbruiklijst)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (dagverbruik.UurLijst[i].UurNummer != i)
                    {
                        throw new Exception("Uurnummer matcht niet in GETSTATS");
                    }
                    
                    stats.UurStatsList[i].UurLabel = dagverbruik.UurLijst[i].UurLabel;
                    int N = dagverbruik.UurLijst[i].AantalMetingen;
                    if (N != 0) {
                        stats.UurStatsList[i].AantalWaarnemingen++;
                    }
                    stats.UurStatsList[i].Uurwaarden.Add(dagverbruik.UurLijst[i].UurVerbruik);
                }
                
                stats.DagStats.Dagwaarden.Add(dagverbruik.TotaalperDag);
                stats.DagStats.Datums.Add(dagverbruik.VerbruiksDatum);
                             
            }
            for (int i = 0; i < 24; i++)
            {
                stats.UurStatsList[i].StatNumbers = GetNumbers(stats.UurStatsList[i].Uurwaarden); 
            }
            stats.DagStats.StatNumbers = GetNumbers(stats.DagStats.Dagwaarden);

            return stats;
        }

        public StatNumbers GetNumbers (List<decimal> getallenlijst )
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
            if (p20up > aantal-1) { p20up = aantal - 1; }
            decimal perc20 = sortedlist[p20down] + ((sortedlist[p20up] - sortedlist[p20down]) * (0.5M));

            decimal p40dec = 40M * (a - 1M) / 100M;
            if (p40dec < 0) { p40dec = 0; }
            if (p40dec >= a - 1M) { p40dec = aantal - 1; }
            int p40down = Convert.ToInt32(Math.Truncate(p40dec));
            int p40up = p40down + 1;
            if (p40up > aantal-1) { p40up = aantal - 1; }
            decimal perc40 = sortedlist[p40down] + ((sortedlist[p40up] - sortedlist[p40down]) * (0.5M));

            decimal p60dec = 60M * (a - 1M) / 100M;
            if (p60dec < 0) { p60dec = 0; }
            if (p60dec >= a - 1M) { p60dec = aantal - 1; }
            int p60down = Convert.ToInt32(Math.Truncate(p60dec));
            int p60up = p60down + 1;
            if (p60up > aantal-1) { p60up = aantal - 1; }
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
    }
     
}