using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class AgvData
    {
        public int Id { get; set; }
        public string Factory { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
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

