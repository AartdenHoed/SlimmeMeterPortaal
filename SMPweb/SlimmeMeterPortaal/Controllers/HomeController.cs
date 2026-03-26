using Microsoft.Ajax.Utilities;
using SlimmeMeterPortaal.ViewModels;
using System;
using System.Drawing;
using System.Management.Automation.Language;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SlimmeMeterPortaal.Controllers
{
    public class HomeController : Controller    {

        private readonly SlimmeMeterPortaalEntities db = new SlimmeMeterPortaalEntities();
        
        public async Task<ActionResult> IndexDag()
        // Startscherm voor dagrapportage
        {
            SmpViewModel SmpViewModel = new SmpViewModel
            {
                IncludeGas = "Y",
                IncludeStroom = "Y",
                SMP_Guid = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SmpViewModel.InitProgressBar(db);
                       
            // Fetch cookie value for input fields, if any
            HttpCookie inputyearsCookie = Request.Cookies["InputYears"];
            if (inputyearsCookie != null)
            {
                SmpViewModel.ReferentieJaren = Int32.Parse(inputyearsCookie.Value);
            }
            HttpCookie inputdaysCookie = Request.Cookies["InputDays"];
            if (inputdaysCookie != null)
            {
                SmpViewModel.ReferentieDagen = Int32.Parse(inputdaysCookie.Value);
            }

            string title = "Dagrapportage";
            string lvl = SmpViewModel.MessageViewModel.Info;
            string msg = "Geef de gevraagde input voor de dagrapportage en klik op 'Maak Dagrapport', of selecteer links het inputscherm voor de maandrapportage";

            Task<string> maakVerbruiksMeterLijst = SmpViewModel.MaakVerbruiksMeterLijst();
            string result = await maakVerbruiksMeterLijst;
            maakVerbruiksMeterLijst.Dispose();

            if (result != "Ok")
            {
                throw new Exception("Het maken van een lijst van verbruiksmeters ging fout, returncode = " + result.ToString());
            }
            string val = SmpViewModel.DatumValidatie(0);
            if (val != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Warning;
                msg = val;
            }

            SmpViewModel.MessageViewModel.Fill(title, lvl, msg);

            // Console.WriteLine("Stuur view");

            return View("IndexDag", SmpViewModel);
        }

        public async Task<ActionResult> IndexMaand()
        {
            SmpViewModel SmpViewModel = new SmpViewModel
            {
                IncludeGas = "Y",
                IncludeStroom = "Y",
                SMP_Guid = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SmpViewModel.InitProgressBar(db);

            // Fetch cookie value for input fields, if any
            HttpCookie inputyearsCookie = Request.Cookies["InputYears"];
            if (inputyearsCookie != null)
            {
                SmpViewModel.ReferentieJaren = Int32.Parse(inputyearsCookie.Value);
            }

            string title = "Maandrapportage";
            string lvl = SmpViewModel.MessageViewModel.Info;
            string msg = "Geef de gevraagde input voor de maandrapportage en klik op 'Maak Maandrapport', of selecteer links het inputscherm voor de dagrapportage";

            Task<string> maakVerbruiksMeterLijst = SmpViewModel.MaakVerbruiksMeterLijst();
            string result = await maakVerbruiksMeterLijst;
            maakVerbruiksMeterLijst.Dispose();

            if (result != "Ok")
            {
               throw new Exception("Het maken van een lijst van verbruiksmeters ging fout, returncode = " + result.ToString());
            }
            string val = SmpViewModel.DatumValidatie(0);
            if (val != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Warning;
                msg = val;
            }
            SmpViewModel.MessageViewModel.Fill(title, lvl, msg);

            // Console.WriteLine("Stuur view");

            return View("IndexMaand", SmpViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DagRapport(SmpViewModel SmpViewModel)
        {

            // Set cookie values according to input screen
            HttpCookie inputdaysCookie = new HttpCookie("InputDays")
            {
                Value = SmpViewModel.ReferentieDagen.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
            HttpCookie inputyearsCookie = new HttpCookie("InputYears")
            {
                Value = SmpViewModel.ReferentieJaren.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
            Response.Cookies.Add(inputdaysCookie);
            Response.Cookies.Add(inputyearsCookie);

            string title = "Dagrapportage";
            string lvl = SmpViewModel.MessageViewModel.Info;
            string msg = "Gebruik de knoppen links, of scroll naar beneden om de gevraagde rapportages te zien";
            string result;

            if (SmpViewModel.VerbruiksMeters.Count == 0)
            {
                Task<string> maakVerbruiksMeterLijst = SmpViewModel.MaakVerbruiksMeterLijst();
                result = await maakVerbruiksMeterLijst;
                maakVerbruiksMeterLijst.Dispose();
                if (result != "Ok")
                {
                    lvl = SmpViewModel.MessageViewModel.Error;
                    msg = "Het maken van een lijst van verbruiksmeters ging fout, returncode = " + result.ToString();
                }
            }
            string val = SmpViewModel.DatumValidatie(1);
            if (val != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Warning;                
                msg = val;
                SmpViewModel.MessageViewModel.Fill(title, lvl, msg);
                return View("IndexDag", SmpViewModel);
            }

            // Create a list of seven days with the raw data from the API site
            Task<string> maakWeekRuweVerbruikLijst = SmpViewModel.MaakDagRuweVerbruiksLijst();
            result = await maakWeekRuweVerbruikLijst;
            maakWeekRuweVerbruikLijst.Dispose();
            if (result != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Error;
                msg = "Het maken van een lijst van dagen met ruwverbruik mislukte met returncode " + result;
            }

            Task<string> maakWeekReferentieRuweVerbruiksLijst = SmpViewModel.MaakDagReferentieRuweVerbruiksLijst();
            result = await maakWeekReferentieRuweVerbruiksLijst;
            maakWeekReferentieRuweVerbruiksLijst.Dispose();
            if (result != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Error;
                msg = "Het maken van een referentielijst van dagen met ruwverbruik mislukte met returncode " + result;
            }

            SmpViewModel.ConsolideerRuweDagVerbruiksData();
            SmpViewModel.BerekenDagRapportStatistieken();

            SmpViewModel.MessageViewModel.Fill(title, lvl, msg);

            SmpViewModel.MaakDagRapport();

            return View("DagRapport", SmpViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MaandRapport(SmpViewModel SmpViewModel)
        {
            // Set cookie values according to input screen

            HttpCookie inputyearsCookie = new HttpCookie("InputYears")
            {
                Value = SmpViewModel.ReferentieJaren.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
                     
            Response.Cookies.Add(inputyearsCookie);

            string title = "Maandrapportage";
            string lvl = SmpViewModel.MessageViewModel.Info;
            string msg = "Gebruik de knoppen links, of scroll naar beneden om de gevraagde rapportages te zien";
            string result;

            if (SmpViewModel.VerbruiksMeters.Count == 0)
            {
                Task<string> maakVerbruiksMeterLijst = SmpViewModel.MaakVerbruiksMeterLijst();
                result = await maakVerbruiksMeterLijst;
                maakVerbruiksMeterLijst.Dispose();
                if (result != "Ok")
                {
                    lvl = SmpViewModel.MessageViewModel.Error;
                    msg = "Het maken van een lijst van verbruiksmeters ging fout, returncode = " + result.ToString();
                }
            }
            string val = SmpViewModel.DatumValidatie(1);
            if (val != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Warning;
                msg = val;
                SmpViewModel.MessageViewModel.Fill(title, lvl, msg);
                return View("IndexMaand", SmpViewModel);
            }

            Task<string> longRunningTask2 = SmpViewModel.GetMonthUsage();
            result = await longRunningTask2;
            longRunningTask2.Dispose();
            if (result != "Ok")
            {
                lvl = SmpViewModel.MessageViewModel.Error;
                msg = "AWAIT GETMONTHUSAGE failed rc = " + result;
            }

            SmpViewModel.GetMaandCijfers();
            
            SmpViewModel.GetMonthStats();

            SmpViewModel.MessageViewModel.Fill(title, lvl, msg);

            SmpViewModel.MaandRapport();

            return View("MaandRapport", SmpViewModel);
        }
    }
}