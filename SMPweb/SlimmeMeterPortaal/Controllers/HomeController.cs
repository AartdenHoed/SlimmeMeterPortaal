using SlimmeMeterPortaal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Web.UI.WebControls;

namespace SlimmeMeterPortaal.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            SMP_Report SMP_report = new SMP_Report();
            
            string title = "Rapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Geef de gevraagde input voor de rapportage";         
                        
            Task<string> longRunningTask = SMP_report.GetMeters();
            string result = await longRunningTask;
            longRunningTask.Dispose();

            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT failed rc = " + result.ToString();
            }
            SMP_report.Message.Fill(title, lvl, msg);

            // Console.WriteLine("Stuur view");

            return View("Index", SMP_report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DagRapport(SMP_Report SMP_report)
        {
            string title = "Rapportage";
            string lvl = SMP_report.Message.Info;
            string msg = "Scroll naar beneden om de gevraagde rapportages te zien";
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

            SMP_Input smp_Input = new SMP_Input();

            Task<string> longRunningTask2 = smp_Input.GetUsage(SMP_report); 
            result = await longRunningTask2;
            longRunningTask2.Dispose();
            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT GETUSAGE failed rc = " + result;
            }

            Task<string> longRunningTask3 = smp_Input.GetReference(SMP_report);
            result = await longRunningTask3;
            longRunningTask3.Dispose();
            if (result != "Ok")
            {
                lvl = SMP_report.Message.Error;
                msg = "AWAIT GETREFERENCE failed rc = " + result;
            }

            smp_Input.Consolidate();
            smp_Input.Statistics(); 

            SMP_report.Message.Fill(title, lvl, msg);
            SMP_report.Create_Report(smp_Input.GasUurLijst, smp_Input.GasStatistieken);
            SMP_report.Create_Report(smp_Input.StroomUurLijst, smp_Input.StroomStatistieken);

            return View("DagRapport", SMP_report);
        }
    }
}