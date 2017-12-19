using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class AgvSupply
    {
        public int Id { get; set; }
        public string Factory { get; set; }
        public string Shift { get; set; }
        public string Date { get; set; }
        public string Dept { get; set; }
        public string Block { get; set; }
        public string AgvName { get; set; }
        public string Part { get; set; }
        public int? Route { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double? SupplyTime { get; set; }
        public double? CycleTimeTarget { get; set; }
        public int? TotalSupplyTarget { get; set; }
        public string Description { get; set; }

        public double? NORMAL { get; set; }
        public double? STOP_BY_CARD { get; set; }
        public double? SAFETY { get; set; }
        public double? BATTERY_EMPTY { get; set; }
        public double? NO_CART { get; set; }
        public double? POLE_ERROR { get; set; }
        public double? OUT_OF_LINE { get; set; }
        public double? EMERGENCY { get; set; }

        public string NORMAL_DETAIL { get; set; }
        public string STOP_BY_CARD_DETAIL { get; set; }
        public string SAFETY_DETAIL { get; set; }
        public string BATTERY_EMPTY_DETAIL { get; set; }
        public string NO_CART_DETAIL { get; set; }
        public string POLE_ERROR_DETAIL { get; set; }
        public string OUT_OF_LINE_DETAIL { get; set; }
        public string EMERGENCY_DETAIL { get; set; }
    }
}
