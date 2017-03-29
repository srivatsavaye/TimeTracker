using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeTrackerLibrary.Models;

namespace TimeTrackerLibrary
{
    public class TimeSheetRepository : ITimeSheetRepository
    {
        private string path = "Timesheets";
        public void Write(string name, string text)
        {
            File.WriteAllText(path + @"\" + name, text);
        }

        public string Read(string name)
        {
            return File.Exists(path + @"\" + name) ? File.ReadAllText(path + @"\" + name) : null;
        }
    }
}
