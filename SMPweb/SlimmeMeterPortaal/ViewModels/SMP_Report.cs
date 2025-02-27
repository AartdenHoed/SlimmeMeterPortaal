using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using SlimmeMeterPortaal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using System.Security.Cryptography.X509Certificates;

namespace SlimmeMeterPortaal.ViewModels
{
    public class SMP_Report

    {
        public MessageVM Message = new MessageVM();

        [Required(ErrorMessage = "Rapportage datum is een verplicht veld")]
        [DisplayName("Rapporteer over de week t/m deze datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime Rapportagedatum { get; set; } = DateTime.Today.AddDays(-1);

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
       
        public string APIkey
        {
            get
            {
                StreamReader sr = new StreamReader("O:\\ADHC Output\\SlimmeMeterPortaal\\SlimmeMeterPortaal.api");
                //Read the first line of text
                string line = sr.ReadLine();
                
                sr.Close();
                return (line.TrimEnd());

            }
        }
        
        public async Task<string> GetMeters()
        {

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
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // Parse the response body
                var meterlist = JsonConvert.DeserializeObject<List<Meter>>(responseString);

                foreach (Meter m in meterlist)
                {
                    DeviceVM dv = new DeviceVM (m.connection_type,this.IncludeGas, this.IncludeStroom)
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
                    this.Devicelijst.Add(dv);
                }

            }
            else
            {
                this.Message.Tekst = response.StatusCode.ToString() + " ---> " + response.ReasonPhrase;
                this.Message.Level = this.Message.Error;
                
            }
            request.Dispose();
            response.Dispose();
            httpClient.Dispose();
            return "Ok";
        }
        public async Task<string> GetUsage()
        {
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                DateTime reportDate = this.Rapportagedatum;
                for (int i = -6; i <= 0; i++)
                {
                    DateTime entrydate = this.Rapportagedatum.AddDays(i);
                    string datestring = entrydate.ToString("dd-MM-yyyy");
                    Task<string> longRunningTask = dvm.GetSMPday(datestring, this.APIkey, "Usage");
                    string result = await longRunningTask;

                    if (result != "Ok")
                    {
                        throw new Exception("Function GetSMPday failed");

                    }
                }

            }
            return "Ok";
        }

        public async Task<string> GetReference()
        {
            foreach (DeviceVM dvm in this.Devicelijst)
            {
                if (!dvm.ReportDevice) { continue; }
                for (int year = -1 * this.ReferentieJaren; year < 0; year++)
                {
                    DateTime refdate = this.Rapportagedatum.AddYears(year);
                    int two = 2;
                    int daymin = -1 * (this.ReferentieDagen - 1) / two;
                    int daymax = daymin + this.ReferentieDagen;
                    for (int day = daymin; day < daymax; day++)
                    {
                        DateTime entrydate = this.Rapportagedatum.AddYears(year).AddDays(day);
                        string datestring = entrydate.ToString("dd-MM-yyyy");
                        Task<string> longRunningTask = dvm.GetSMPday(datestring, this.APIkey,"Reference");
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
    }
}