using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class MessageVM
    {
        public string Error { get { return "E"; } }
        public string Warning { get { return "W"; } }
        public string Info { get { return "I"; } }
        public string Tekst { get; set; }
        public string Level { get; set; }
        public string Title { get; set; }

        public void Fill(string title, string lvl, string msg)
        {
            this.Tekst = msg;
            this.Level = lvl;
            this.Title = title;


        }
    }
       
}