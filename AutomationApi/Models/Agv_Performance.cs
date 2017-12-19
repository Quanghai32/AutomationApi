using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class Agv_Performance
    {
        public int Id { get; set; }
        public string Factory { get; set; }
        public string Shift { get; set; }
        public DateTime Date { get; set; }
        public string Dept { get; set; }
        public string Block { get; set; }
        public double Operation_Rate { get; set; }
        public double SupplyPart_Rate { get; set; }
    }
}
