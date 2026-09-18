using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlimmeMeterPortaal.ViewModels
{
    public class MessageViewModel
    {
        public string Error { get { return "E"; } }
        public string Warning { get { return "W"; } }
        public string Info { get { return "I"; } }
        public string Tekst { get; set; }
        public string Level { get; set; } = "O";
        public string Title { get; set; }

        public void Fill(string title, string lvl, string msg)
        {
            this.Title = title;
            // first message "wins"
            if (this.Level == "O")
            {
                this.Level = lvl;
                if ((this.Level == "W") || (this.Level == "E")) {
                    this.Tekst = this.Title + " *** " + msg;
                }
                else {
                    this.Tekst = msg;
                }
            }
        }
    }
       
}