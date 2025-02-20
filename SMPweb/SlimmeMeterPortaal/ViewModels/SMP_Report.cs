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
        
        public List<RPT_line> RPT_lines = new List<RPT_line>();

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
                    DeviceVM dv = new DeviceVM
                    {
                        Startdate = DateTime.ParseExact(m.start_date, "dd-MM-yyyy", null),
                        DeviceID = m.meter_identifier,
                        DeviceType = m.connection_type,
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

        public void Create_Report(List<DagVerbruik> dagverbruik, Stats stats)
        {
            // Titles
            RPT_line line = new RPT_line
            {
                N = "N",
                Min = "Min",
                Mean = "Mean",
                Max = "Max",
                Label = "Uur",
                Dag1 = dagverbruik[0].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag2 = dagverbruik[1].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag3 = dagverbruik[2].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag4 = dagverbruik[3].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag5 = dagverbruik[4].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag6 = dagverbruik[5].VerbruiksDatum.ToString("yyyy-MM-dd"),
                Dag7 = dagverbruik[6].VerbruiksDatum.ToString("yyyy-MM-dd")
            };
            this.RPT_lines.Add(line);

            for (int i=0; i < 24; i++)
            {
                RPT_line lined = new RPT_line
                {
                    N = stats.UurStatsList[i].AantalWaarnemingen.ToString("D2"),
                    Min = stats.UurStatsList[i].StatNumbers.Min.ToString("N2"),
                    Mean = stats.UurStatsList[i].StatNumbers.Mean.ToString("N2"),
                    Max = stats.UurStatsList[i].StatNumbers.Max.ToString("N2"),
                    Label = stats.UurStatsList[i].UurLabel,
                    Dag1 = dagverbruik[0].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag2 = dagverbruik[1].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag3 = dagverbruik[2].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag4 = dagverbruik[3].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag5 = dagverbruik[4].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag6 = dagverbruik[5].UurLijst[i].UurVerbruik.ToString("N2"),
                    Dag7 = dagverbruik[6].UurLijst[i].UurVerbruik.ToString("N2"),
                    Level1 = stats.GetLevel(dagverbruik[0].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level2 = stats.GetLevel(dagverbruik[1].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level3 = stats.GetLevel(dagverbruik[2].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level4 = stats.GetLevel(dagverbruik[3].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level5 = stats.GetLevel(dagverbruik[4].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level6 = stats.GetLevel(dagverbruik[5].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers),
                    Level7 = stats.GetLevel(dagverbruik[6].UurLijst[i].UurVerbruik, stats.UurStatsList[i].StatNumbers)
                };
                this.RPT_lines.Add(lined);

            }
            RPT_line linex = new RPT_line
            {
                N = stats.DagStats.AantalWaarnemingen.ToString("D2"),
                Min = stats.DagStats.StatNumbers.Min.ToString("N2"),
                Mean = stats.DagStats.StatNumbers.Mean.ToString("N2"),
                Max = stats.DagStats.StatNumbers.Max.ToString("N2"),
                Label = "Dag Totaal",
                Dag1 = dagverbruik[0].TotaalperDag.ToString("N2"),
                Dag2 = dagverbruik[1].TotaalperDag.ToString("N2"),
                Dag3 = dagverbruik[2].TotaalperDag.ToString("N2"),
                Dag4 = dagverbruik[3].TotaalperDag.ToString("N2"),
                Dag5 = dagverbruik[4].TotaalperDag.ToString("N2"),
                Dag6 = dagverbruik[5].TotaalperDag.ToString("N2"),
                Dag7 = dagverbruik[6].TotaalperDag.ToString("N2"),
                Level1 = stats.GetLevel(dagverbruik[0].TotaalperDag, stats.DagStats.StatNumbers),
                Level2 = stats.GetLevel(dagverbruik[1].TotaalperDag, stats.DagStats.StatNumbers),
                Level3 = stats.GetLevel(dagverbruik[2].TotaalperDag, stats.DagStats.StatNumbers),
                Level4 = stats.GetLevel(dagverbruik[3].TotaalperDag, stats.DagStats.StatNumbers),
                Level5 = stats.GetLevel(dagverbruik[4].TotaalperDag, stats.DagStats.StatNumbers),
                Level6 = stats.GetLevel(dagverbruik[5].TotaalperDag, stats.DagStats.StatNumbers),
                Level7 = stats.GetLevel(dagverbruik[6].TotaalperDag, stats.DagStats.StatNumbers)
            };
            this.RPT_lines.Add(linex);

            return;
        }



       
    }
}