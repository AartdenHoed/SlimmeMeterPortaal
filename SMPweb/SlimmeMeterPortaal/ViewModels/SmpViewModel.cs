using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SlimmeMeterPortaal.ViewModels
{
   
    public class SmpViewModel

    {
        // Input screen variables
        public DateTime RapportageDatum { get
            {
                // Screen input  via RapportageDatumString because that makes validation much easier
                string[] formats = { "yyyy-MM-dd" };
                DateTime result;
                if (DateTime.TryParseExact(this.RapportageDatumString, formats,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime testdate))
                {
                    result = testdate;
                }
                else
                {
                    result = DateTime.MinValue;
                }
                return result;
            }
            set { }
        }

        [Required(ErrorMessage = "Rapportage datum is een verplicht veld, format yyyy-mm-dd")]
        [StringLength(10)]
        [DisplayName("Rapporteer t/m deze datum")]    
        public string RapportageDatumString { get; set; }
       
        [Required(ErrorMessage = "Referentie jaren is een verplicht veld")]
        [DisplayName("Aantal jaren historie ter vergelijking")]
        public int ReferentieJaren { get; set; } = 5;

        [Required(ErrorMessage = "Referentie dagen is een verplicht veld")]
        [DisplayName("Aantal dagen per referentiejaar ter vergelijking")]
        public int ReferentieDagen { get; set; } = 20;

        public MessageViewModel MessageViewModel = new MessageViewModel();

        // List of meters
        public List<VerbruiksMeter> VerbruiksMeters = new List<VerbruiksMeter>();  
        
        // Which meters to include
        public string IncludeStroom { get; set; }
        public string IncludeGas { get; set; }
        
        // Variables needed for progress bar (not yet implemented)
        public string SMP_Guid { get; set; }
        public DateTime Timestamp { get; set; }

        public string URL = "https://app.slimmemeterportal.nl/userapi/v1/connections";
        public string APIkey
        {
            // Fetch API key from dataset to prevent inclusion in coding
            get
            {
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                Pipeline pipeline = runspace.CreatePipeline();

                //RUN FIXED INITVAR from production
                
                Command myCommand = new Command("C:\\ADHC\\Powershell\\INITVAR.PS1");
                CommandParameter testParam = new CommandParameter("MODE", "JSON");
                myCommand.Parameters.Add(testParam);

                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> resultobj = pipeline.Invoke();

                string json = resultobj[0].ToString();
                string apikey = JObject.Parse(json)["ADHC_SMPapikey"].ToString();   

                return (apikey);

            }
        }

// Hieronder alle algemene methods
        public void InitProgressBar(SlimmeMeterPortaalEntities db)
        {
            DateTime now = DateTime.Now.AddDays(-1);

            try
            {
                var query = from progressbar in db.ProgressBars
                            where progressbar.Timestamp < now
                            select progressbar;
                List < ProgressBar > Oldbars = query.ToList();

                if (Oldbars.Count > 0)
                {
                    foreach (ProgressBar pb in Oldbars)
                    {
                        db.ProgressBars.Remove(pb);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                this.MessageViewModel.Tekst = e.Message;
                this.MessageViewModel.Level = this.MessageViewModel.Error;

            }        
           
            
            return;
        }

        public string DatumValidatie(int type)
        {
            string result = "Ok";

            // Determine maximum date
            DateTime maxd = DateTime.MinValue;
            foreach (VerbruiksMeter dv in this.VerbruiksMeters)
            {
                if (dv.LastDateWithData > maxd)
                {
                    maxd = dv.LastDateWithData;
                }
            }
            // Determine minimum date
            DateTime mind = DateTime.MinValue;
            foreach (VerbruiksMeter dv in this.VerbruiksMeters)
            {
                if (dv.Startdate > mind)
                {
                    mind = dv.Startdate;
                }
            }
            int miny = mind.Year;
            mind = new DateTime(miny + 1, 1, 1);

            if (string.IsNullOrEmpty(this.RapportageDatumString))
                if (type == 0)  
                {                    
                    this.RapportageDatum = maxd;
                    this.RapportageDatumString = maxd.ToString("yyyy-MM-dd");
                }
                else
                {
                    result = "Specificeer een datum in formaat yyyy-mm-dd";
                }
            else 
            {
                string[] formats = { "yyyy-MM-dd" };                
                if (!DateTime.TryParseExact(this.RapportageDatumString, formats,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out DateTime testdate))
                {
                    result = "Ongeldige datum - geef datum in het format yyyy-MM-dd.";
                }
                else
                {
                    this.RapportageDatum = testdate;
                    if (this.RapportageDatum > maxd)
                    {
                        result = "Van deze rapportagedatum hebben we nog geen data";
                    }
                    if (this.RapportageDatum.AddYears(-1 * this.ReferentieJaren) < mind)
                    {
                        result = "Op deze rapportagedatum hebben we nog niet " + this.ReferentieJaren.ToString("d2") + " referentiejaren beschikbaar.";
                    }
                }
            }
          

            return result;
        }

        public async Task<string> MaakVerbruiksMeterLijst()
        {           
            int retrycount = 0;
            string result = "Nok";
            string result2 = "x"; 
            do { 
                

                //#$strdate = "14-01-2024"
                //#$meterid = "871689290200620802"
                //$url = "https://app.slimmemeterportal.nl/userapi/v1/connections/" + $meterID + "/usage/" + $strdate

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage
                {
                    RequestUri = new Uri(this.URL),
                    Method = HttpMethod.Get
                };
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Properties.Add("Content-Type", "application/json");
                request.Headers.Add("API-key", this.APIkey);
               
                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)  
                {
                    this.MessageViewModel.Tekst = response.StatusCode.ToString() + " ---> " + response.ReasonPhrase;
                    this.MessageViewModel.Level = this.MessageViewModel.Error;
                    retrycount += 1;
                    Thread.Sleep(5000);
                }
                else
                {
                    result = "Ok";
                    var responseString = await response.Content.ReadAsStringAsync();
                    // Parse the response body
                    var meterlist = JsonConvert.DeserializeObject<List<RawMeter>>(responseString);

                    foreach (RawMeter m in meterlist)
                    {
                        VerbruiksMeter dv = new VerbruiksMeter(m.connection_type, this.IncludeGas, this.IncludeStroom)
                        {
                            Startdate = DateTime.ParseExact(m.start_date, "dd-MM-yyyy", null),
                            MeterIdentificatie = m.meter_identifier
                        };
                        if (!string.IsNullOrEmpty(m.end_date))
                        {
                            dv.Enddate = DateTime.ParseExact(m.end_date, "dd-MM-yyyy", null);
                        }
                        else
                        {
                            dv.Enddate = DateTime.MaxValue;
                        }

                        // Get latest date with data for this meter
                        DateTime ldate = DateTime.Now;
                        bool ldatefound = false;

                        do
                        {
                            string datestring = ldate.ToString("dd-MM-yyyy");
                            Task<string> longRunningTask = dv.HaalEenDagRuwVerbruik(datestring, this.APIkey, "Lastdate");
                            result2 = await longRunningTask;

                            if (result2 == "Last")
                            {
                                ldatefound = true;
                                dv.LastDateWithData = ldate;
                                
                            }

                            ldate = ldate.AddDays(-1);

                        } while (!ldatefound);


                        this.VerbruiksMeters.Add(dv);
                    }
                    request.Dispose();
                    response.Dispose();
                    httpClient.Dispose();
                }
            } while ((result != "Ok") && (retrycount < 10));
              
            return result;
        }
 
        
// Hieronder alle methods voor de dagrapportage
        public async Task<string> MaakDagRuweVerbruiksLijst()
        {
            string result = "Nok";
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                
                for (int i = -6; i <= 0; i++)
                {
                    DateTime entrydate = this.RapportageDatum.AddDays(i);
                    string datestring = entrydate.ToString("dd-MM-yyyy");
                    Task<string> longRunningTask = dvm.HaalEenDagRuwVerbruik(datestring, this.APIkey, "Usage");
                    result = await longRunningTask;

                }

            }
            return result;
        }

        public async Task<string> MaakDagReferentieRuweVerbruiksLijst()
        {
            string result = "Nok";
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                for (int year = -1 * this.ReferentieJaren; year < 0; year++)
                {
                    
                    int two = 2;
                    int daymin = -1 * (this.ReferentieDagen - 1) / two;
                    int daymax = daymin + this.ReferentieDagen;
                    
                    for (int day = daymin; day < daymax; day++)
                    {
                        DateTime entrydate = this.RapportageDatum.AddYears(year).AddDays(day);
                        string datestring = entrydate.ToString("dd-MM-yyyy");
                                   
                        Task<string> longRunningTask = dvm.HaalEenDagRuwVerbruik(datestring, this.APIkey, "Reference");
                        result = await longRunningTask;
                        
                    }
                }
            }
            return result;
        }

        public void ConsolideerRuweDagVerbruiksData()
        {
            // Consolideer de data in de ruwe verbruiksdata. Doet dit voor zowel de te rapporteren dagen, als voor de referentie dagen 
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                switch (dvm.MeterType)
                {
                    case "gas":
                        dvm.GasConsolidatie("Usage");
                        dvm.GasConsolidatie("Reference");
                        break;
                    case "elektriciteit":
                        dvm.StroomConsolidatie("Usage");
                        dvm.StroomConsolidatie("Reference");
                        break;
                }
            }
            return;
        }

        public void BerekenDagRapportStatistieken()
        {
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)                
            {
                if (!dvm.MeterRapportMaken) { continue; }
                dvm.BerekenDagRapportStatistieken();                
            }
            return;            
        }

        public void MaakDagRapport()
        {
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                dvm.Create_DagRapport();
            }
            return;
        }

// Hieronder de methodes voor de maandrapportage
        public async Task<string> GetMonthUsage()
        {
            string result = "Nok";
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                if (dvm.LastDateWithData < this.RapportageDatum)
                {
                    dvm.RapportageDatum = dvm.LastDateWithData;
                }
                else
                {
                    dvm.RapportageDatum = this.RapportageDatum;
                }
                int firstYear = this.RapportageDatum.AddYears(-1 * this.ReferentieJaren).Year;
                int currentYear = this.RapportageDatum.Year;
                for (int i = firstYear; i <= currentYear; i++)
                {
                    
                    Task<string> longRunningTask = dvm.GetSMPmonth(i, this.APIkey);
                    result = await longRunningTask;
                                      
                }

            }
            return result;
        }

        public void GetMaandCijfers()       
        {
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                switch (dvm.MeterType)
                {
                    case "gas":
                        dvm.GasMaandCijfers();
                        break;
                    case "elektriciteit":
                        dvm.StroomMaandCijfers();
                        break;
                }
            }
            return;
        }

        public void GetMonthStats()
        {
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                dvm.MonthStats();
            }
            return;
        }

        public void MaandRapport()
        {
            foreach (VerbruiksMeter dvm in this.VerbruiksMeters)
            {
                if (!dvm.MeterRapportMaken) { continue; }
                dvm.Create_MaandRapport();
            }
            return;
        }
    }
}