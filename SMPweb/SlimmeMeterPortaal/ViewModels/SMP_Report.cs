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
   
    public class SMP_Report

    {
        public MessageVM Message = new MessageVM();

        [Required(ErrorMessage = "Rapportage datum is een verplicht veld")]
        [DisplayName("Rapporteer t/m deze datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime Rapportagedatum { get; set; }

        [Required(ErrorMessage = "Rapportage datum is een verplicht veld, format yyyy-mm-dd")]
        [StringLength(10)]
        [DisplayName("Rapporteer t/m deze datum")]    
        public string Rapportagestringdatum { get; set; }
       
        [Required(ErrorMessage = "Referentie jaren is een verplicht veld")]
        [DisplayName("Aantal jaren historie ter vergelijking")]
        public int ReferentieJaren { get; set; } = 5;

        [Required(ErrorMessage = "Referentie dagen is een verplicht veld")]
        [DisplayName("Aantal dagen per referentiejaar ter vergelijking")]
        public int ReferentieDagen { get; set; } = 20;  
        
        public List<DeviceVM> Devicelijst = new List<DeviceVM>();

        public string URL = "https://app.slimmemeterportal.nl/userapi/v1/connections";

        public string IncludeStroom { get; set; }
        public string IncludeGas { get; set; }
               
        public string SMP_Guid { get; set; }

        public DateTime Timestamp { get; set; }
         
        public string APIkey
        {
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

        public void CleanDB(SlimmeMeterPortaalEntities db)
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
                this.Message.Tekst = e.Message;
                this.Message.Level = this.Message.Error;

            }        
           
            
            return;
        }

        public string DatumValidatie(int type)
        {
            string result = "Ok";

            // Determine maximum date
            DateTime maxd = DateTime.MinValue;
            foreach (DeviceVM dv in this.Devicelijst)
            {
                if (dv.LastDateWithData > maxd)
                {
                    maxd = dv.LastDateWithData;
                }
            }
            // Determine minimum date
            DateTime mind = DateTime.MinValue;
            foreach (DeviceVM dv in this.Devicelijst)
            {
                if (dv.Startdate > mind)
                {
                    mind = dv.Startdate;
                }
            }
            int miny = mind.Year;
            mind = new DateTime(miny + 1, 1, 1);

            if (string.IsNullOrEmpty(this.Rapportagestringdatum))
                if (type == 0)  
                {                    
                    this.Rapportagedatum = maxd;
                    this.Rapportagestringdatum = maxd.ToString("yyyy-MM-dd");
                }
                else
                {
                    result = "Specificeer een datum in formaat yyyy-mm-dd";
                }
            else 
            {
                string[] formats = { "yyyy-MM-dd" };
                DateTime testdate;
                if (!DateTime.TryParseExact(this.Rapportagestringdatum, formats,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out testdate))
                {
                    result = "Ongeldige datum - geef datum in het format yyyy-MM-dd.";
                }
                else
                {
                    this.Rapportagedatum = testdate;
                    if (this.Rapportagedatum > maxd)
                    {
                        result = "Van deze rapportagedatum hebben we nog geen data";
                    }
                    if (this.Rapportagedatum.AddYears(-1 * this.ReferentieJaren) < mind)
                    {
                        result = "Op deze rapportagedatum hebben we nog niet " + this.ReferentieJaren.ToString("d2") + " referentiejaren beschikbaar.";
                    }
                }
            }
          

            return result;
        }

        public async Task<string> GetMeters()
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
                    this.Message.Tekst = response.StatusCode.ToString() + " ---> " + response.ReasonPhrase;
                    this.Message.Level = this.Message.Error;
                    retrycount += 1;
                    Thread.Sleep(5000);
                }
                else
                {
                    result = "Ok";
                    var responseString = await response.Content.ReadAsStringAsync();
                    // Parse the response body
                    var meterlist = JsonConvert.DeserializeObject<List<Meter>>(responseString);

                    foreach (Meter m in meterlist)
                    {
                        DeviceVM dv = new DeviceVM(m.connection_type, this.IncludeGas, this.IncludeStroom)
                        {
                            Startdate = DateTime.ParseExact(m.start_date, "dd-MM-yyyy", null),
                            DeviceID = m.meter_identifier
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
                            Task<string> longRunningTask = dv.GetSMPday(datestring, this.APIkey, "Lastdate");
                            result2 = await longRunningTask;

                            if (result2 == "Last")
                            {
                                ldatefound = true;
                                dv.LastDateWithData = ldate;
                                
                            }

                            ldate = ldate.AddDays(-1);

                        } while (!ldatefound);


                        this.Devicelijst.Add(dv);
                    }
                    request.Dispose();
                    response.Dispose();
                    httpClient.Dispose();
                }
            } while ((result != "Ok") && (retrycount < 10));
              
            return result;
        }
       
        public async Task<string> GetUsage()
        {
            string result = "Nok";
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                
                for (int i = -6; i <= 0; i++)
                {
                    DateTime entrydate = this.Rapportagedatum.AddDays(i);
                    string datestring = entrydate.ToString("dd-MM-yyyy");
                    Task<string> longRunningTask = dvm.GetSMPday(datestring, this.APIkey, "Usage");
                    result = await longRunningTask;

                }

            }
            return result;
        }

        public async Task<string> GetReference()
        {
            string result = "Nok";
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                for (int year = -1 * this.ReferentieJaren; year < 0; year++)
                {
                    
                    int two = 2;
                    int daymin = -1 * (this.ReferentieDagen - 1) / two;
                    int daymax = daymin + this.ReferentieDagen;
                    
                    for (int day = daymin; day < daymax; day++)
                    {
                        DateTime entrydate = this.Rapportagedatum.AddYears(year).AddDays(day);
                        string datestring = entrydate.ToString("dd-MM-yyyy");
                                   
                        Task<string> longRunningTask = dvm.GetSMPday(datestring, this.APIkey, "Reference");
                        result = await longRunningTask;
                        
                    }
                }
            }
            return result;
        }

        public void Consolidate()
        {
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                switch (dvm.DeviceType)
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

        public void Statistics()
        {
            foreach (DeviceVM dvm in this.Devicelijst)                
            {
                if (!dvm.ReportDevice) { continue; }
                dvm.GetStats();                
            }
            return;            
        }

        public void DagRapport()
        {
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                dvm.Create_DagRapport();
            }
            return;
        }

        public async Task<string> GetMonthUsage()
        {
            string result = "Nok";
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                if (dvm.LastDateWithData < this.Rapportagedatum)
                {
                    dvm.RapportageDatum = dvm.LastDateWithData;
                }
                else
                {
                    dvm.RapportageDatum = this.Rapportagedatum;
                }
                int firstYear = this.Rapportagedatum.AddYears(-1 * this.ReferentieJaren).Year;
                int currentYear = this.Rapportagedatum.Year;
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
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                switch (dvm.DeviceType)
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
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                dvm.MonthStats();
            }
            return;
        }

        public void MaandRapport()
        {
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                dvm.Create_MaandRapport();
            }
            return;
        }
    }
}