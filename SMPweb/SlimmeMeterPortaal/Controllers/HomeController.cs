using SlimmeMeterPortaal.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SlimmeMeterPortaal.Controllers
{
    public class HomeController : Controller    {

        private readonly SlimmeMeterPortaalEntities db = new SlimmeMeterPortaalEntities();
        
        public async Task<ActionResult> Index()
        {
            SMP_Report SMP_report = new SMP_Report
            {
                IncludeGas = "Y",
                IncludeStroom = "Y",
                SMP_Guid = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SMP_report.CleanDB(db);
                       
            // Fetch cookie value for input fields, if any
            HttpCookie inputyearsCookie = Request.Cookies["InputYears"];
            if (inputyearsCookie != null)
            {
                SMP_report.ReferentieJaren = Int32.Parse(inputyearsCookie.Value);
            }
            HttpCookie inputdaysCookie = Request.Cookies["InputDays"];
            if (inputdaysCookie != null)
            {
                SMP_report.ReferentieDagen = Int32.Parse(inputdaysCookie.Value);
            }

            string title = "Dagrapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Geef de gevraagde input voor de dagrapportage, of selecteer links de maandrapportage";

            Task<string> longRunningTask = SMP_report.GetMeters();
            string result = await longRunningTask;
            longRunningTask.Dispose();

            if (result != "Ok")
            {
                throw new Exception("AWAIT failed rc = " + result.ToString());
            }
            string val = SMP_report.DatumValidatie(0);
            if (val != "Ok")
            {
                lvl = SMP_report.Message.Warning;
                msg = val;
            }

            SMP_report.Message.Fill(title, lvl, msg);

            // Console.WriteLine("Stuur view");

            return View("Index", SMP_report);
        }

        public async Task<ActionResult> IndexM()
        {
            SMP_Report SMP_report = new SMP_Report
            {
                IncludeGas = "Y",
                IncludeStroom = "Y",
                SMP_Guid = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SMP_report.CleanDB(db);

            // Fetch cookie value for input fields, if any
            HttpCookie inputyearsCookie = Request.Cookies["InputYears"];
            if (inputyearsCookie != null)
            {
                SMP_report.ReferentieJaren = Int32.Parse(inputyearsCookie.Value);
            }

            string title = "Maandrapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Geef de gevraagde input voor de maandrapportage, of selecteer links de dagrapportage";

            Task<string> longRunningTask = SMP_report.GetMeters();
            string result = await longRunningTask;
            longRunningTask.Dispose();

            if (result != "Ok")
            {
               throw new Exception("AWAIT failed rc = " + result.ToString());
            }
            string val = SMP_report.DatumValidatie(0);
            if (val != "Ok")
            {
                lvl = SMP_report.Message.Warning;
                msg = val;
            }
            SMP_report.Message.Fill(title, lvl, msg);

            // Console.WriteLine("Stuur view");

            return View("IndexM", SMP_report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DagRapport(SMP_Report SMP_report)
        {

            // Set cookie values according to input screen
            HttpCookie inputdaysCookie = new HttpCookie("InputDays")
            {
                Value = SMP_report.ReferentieDagen.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
            HttpCookie inputyearsCookie = new HttpCookie("InputYears")
            {
                Value = SMP_report.ReferentieJaren.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
            Response.Cookies.Add(inputdaysCookie);
            Response.Cookies.Add(inputyearsCookie);

            string title = "Dagrapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Gebruik de knoppen links, of scroll naar beneden om de gevraagde rapportages te zien";
            string result;

            if (SMP_report.Devicelijst.Count == 0)
            {
                Task<string> longRunningTask1 = SMP_report.GetMeters();
                result = await longRunningTask1;
                longRunningTask1.Dispose();
                if (result != "Ok")
                {
                    lvl = SMP_report.Message.Error;
                    msg = "AWAIT GETMETERS failed code = " + result;
                }
            }
            string val = SMP_report.DatumValidatie(1);
            if (val != "Ok")
            {
                lvl = SMP_report.Message.Warning;                
                msg = val;
                SMP_report.Message.Fill(title, lvl, msg);
                return View("Index", SMP_report);
            }

            Task<string> longRunningTask2 = SMP_report.GetUsage();
            result = await longRunningTask2;
            longRunningTask2.Dispose();
            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT GETUSAGE failed rc = " + result;
            }

            Task<string> longRunningTask3 = SMP_report.GetReference();
            result = await longRunningTask3;
            longRunningTask3.Dispose();
            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT GETREFERENCE failed rc = " + result;
            }

            SMP_report.Consolidate();
            SMP_report.Statistics();

            SMP_report.Message.Fill(title, lvl, msg);

            SMP_report.DagRapport();

            return View("DagRapport", SMP_report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MaandRapport(SMP_Report SMP_report)
        {
            // Set cookie values according to input screen

            HttpCookie inputyearsCookie = new HttpCookie("InputYears")
            {
                Value = SMP_report.ReferentieJaren.ToString(),
                HttpOnly = true,
                Path = "/",
                Expires = DateTime.Now.AddDays(366)
            };
                     
            Response.Cookies.Add(inputyearsCookie);

            string title = "Maandrapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Gebruik de knoppen links, of scroll naar beneden om de gevraagde rapportages te zien";
            string result;

            if (SMP_report.Devicelijst.Count == 0)
            {
                Task<string> longRunningTask1 = SMP_report.GetMeters();
                result = await longRunningTask1;
                longRunningTask1.Dispose();
                if (result != "Ok")
                {
                    lvl = SMP_report.Message.Error;
                    msg = "AWAIT GETMETERS failed code = " + result;
                }
            }
            string val = SMP_report.DatumValidatie(1);
            if (val != "Ok")
            {
                lvl = SMP_report.Message.Warning;
                msg = val;
                SMP_report.Message.Fill(title, lvl, msg);
                return View("IndexM", SMP_report);
            }

            Task<string> longRunningTask2 = SMP_report.GetMonthUsage();
            result = await longRunningTask2;
            longRunningTask2.Dispose();
            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT GETMONTHUSAGE failed rc = " + result;
            }

            SMP_report.GetMaandCijfers();
            
            SMP_report.GetMonthStats();

            SMP_report.Message.Fill(title, lvl, msg);

            SMP_report.MaandRapport();

            return View("MaandRapport", SMP_report);
        }
    }
}