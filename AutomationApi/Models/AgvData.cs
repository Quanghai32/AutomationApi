using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class AgvData
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Dept { get; set; }
        public string Block { get; set; }
        public string Name { get; set; }

        public double Safety { get; set; }
        public double Stop_By_Card { get; set; }
        public double Out_Of_Line { get; set; }
        public double No_Cart { get; set; }
        public double Normal { get; set; }
        public double Free { get; set; }
        public double Pole_Error { get; set; }
        public double Disconnect { get; set; }
        public double Shutdown { get; set; }
        public double Total { get; set; }
        public double Battery_Empty { get; set; }
        public double Emergency { get; set; }
    }
}

    //"Id": "0",
    //"Name": "AGV44",
    //"EMG": "7.24517274000002",
    //"Safety": "29.0345109866662",
    //"Stop": "39.1170687083342",
    //"Out line": "1.55870273166667",
    //"Battery empty": "0",
    //"No cart": "1.87200328833334",
    //"Normal": "213.153574401651",
    //"Free": "16.3363486816667",
    //"Pole error": "1.01712178833333",
    //"Disconnect": "140.223966283341",
    //"Shutdown": "5.71871004500315"
