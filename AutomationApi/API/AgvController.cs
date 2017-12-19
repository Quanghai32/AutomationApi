using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutomationApi.Models;
using Newtonsoft.Json;
using System.Collections;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Internal;
using AutomationApi.Datas;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AutomationApi.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class AgvController : Controller
    {
        class OperationRate
        {
            public string Date { get; set; }
            public float Rate { get; set; }
        }
        class Part
        {
            public string Name { get; set; }
            public int? Target { get; set; }
            public double? CycleTime { get; set; }
            public int Count { get; set; }
        }
        class PartCount
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public readonly DataAnalyzeContext _dBContext;
        public AgvController(DataAnalyzeContext dbContext)
        {
            _dBContext = dbContext;
        }

        [HttpGet("GetPerformanceDashboard/{_fact}/{_dept}/{_block}/{_fromdate}/{_todate}/{_isDay}/{_isNight}")]
        public JsonResult GetPerformanceDashboard(string _fact, string _dept, string _block, string _fromdate, string _todate, bool _isDay, bool _isNight)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            var query = _dBContext.Agv_Performances
                .Where(d => d.Factory == _fact && d.Dept == _dept && d.Block == _block && ((day != "") ? d.Shift == day : true))
                .GroupBy(d => new { d.Date, d.Factory, d.Dept, d.Block })
                .Select(d => new
                {
                    Factory = d.Select(e => e.Factory).First(),
                    Dept = d.Select(e => e.Dept).First(),
                    Block = d.Select(e => e.Block).First(),
                    Date = d.Select(e => e.Date.ToString("yyyy-MMM-dd")).First(),
                    Shift = d.Select(e => e.Shift).First(),
                    Operation_Rate = Math.Round(d.Average(e => e.Operation_Rate), 1),
                    SupplyPart_Rate = Math.Round(d.Average(e => e.SupplyPart_Rate), 1)
                })
                //.OrderBy(d => d.Date)
                .ToList();
            return Json(query, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet]
        [Route("GetDeptList")]
        public string GetDeptList()
        {
            var deptList = _dBContext.Factorys.Include(d => d.Depts)
                .ThenInclude(d => d.Blocks)
                .Select(d => d).ToList();
            //return Json(deptList, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return JsonConvert.SerializeObject(deptList, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
        }

        [HttpGet("GetAgvList/{_fact}/{_dept}/{_block}/{_date}/{_isDay}/{_isNight}")]
        public JsonResult GetAgvList(string _fact, string _dept, string _block, string _date, bool _isDay, bool _isNight)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            var query = _dBContext.AgvDatas
                .Where(d => d.Factory == _fact && d.Dept == _dept && d.Block == _block && d.Date == _date)
                .Select(d => d.Name)
                .Distinct()
                .ToList();
            return Json(query, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet("GetAgvOperationRate/{_fact}/{_dept}/{_block}/{_date}/{_isDay}/{_isNight}")]
        public JsonResult GetAgvOperationRate(string _fact, string _dept, string _block, string _date, bool _isDay, bool _isNight)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            var agvRate = _dBContext.AgvDatas.Where(d => d.Factory == _fact && d.Dept == _dept && d.Block == _block && d.Date == _date && ((day != "") ? d.Shift == day : true))
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Normal,
                    i.Battery_Empty,
                    i.Stop_By_Card,
                    i.Shutdown,
                    i.Safety,
                    i.Pole_Error,
                    i.No_Cart,
                    i.Emergency,
                    i.Free,
                    i.Disconnect,
                    i.Out_Of_Line,
                    i.Total,
                })
                .ToList();
            if (agvRate.Count == 0) return Json("");
            return Json(agvRate, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //them tong so lan cap cua block
        [HttpGet("GetAgvSupply/{_fact}/{_dept}/{_block}/{_date}/{_isDay}/{_isNight}")]
        public JsonResult GetAgvSupply(string _fact, string _dept, string _block, string _date, bool _isDay, bool _isNight)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            var agvSupplyTime = _dBContext.AgvSupplys
                .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && ((day != "") ? d.Shift == day : true))
                .Select(d => d.AgvName)
                .GroupBy(d => d)
                .Select(g => new
                {
                    AgvName = g.Distinct().First(),
                    TotalSupply = g.Count()
                });
            var totalSupply = _dBContext.AgvSupplys
                 .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && ((day != "") ? d.Shift == day : true))
                 .Select(d => new { d.Part, d.TotalSupplyTarget })
                 .GroupBy(d => d.Part)
                 .Select(d => d.First())
                 .Sum(d => d.TotalSupplyTarget);

            return Json(agvSupplyTime, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //them target time
        [HttpGet("GetSupplyTime/{_fact}/{_dept}/{_block}/{_date}/{_isDay}/{_isNight}/{_agv}")]
        public JsonResult GetSupplyTime(string _fact, string _dept, string _block, string _date, bool _isDay, bool _isNight, string _agv)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            var agvSupplyTime = _dBContext.AgvSupplys
                .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && ((day != "") ? d.Shift == day : true))
                .Select(i => new
                {
                    i.AgvName,
                    i.SupplyTime,
                    i.StartTime,
                    i.EndTime,
                    i.Route,
                    i.Part,
                    i.Description,
                    i.CycleTimeTarget
                })
                .OrderBy(r => r.Route)
                .ToList();
            if (agvSupplyTime.Count == 0) return Json("");
            return Json(agvSupplyTime, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //
        [HttpGet("GetAgvSupplyOperation/{_fact}/{_dept}/{_block}/{_date}/{_agv}/{_startTime}")]
        public JsonResult GetAgvSupplyOperation(string _fact, string _dept, string _block, string _date, string _agv, string _startTime)
        {
            var agvSupply = _dBContext.AgvSupplys
                .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                .Select(i => new
                {
                    i.AgvName,
                    i.SupplyTime,
                    i.NORMAL,
                    i.STOP_BY_CARD,
                    i.SAFETY,
                    i.BATTERY_EMPTY,
                    i.EMERGENCY,
                    i.NO_CART,
                    i.POLE_ERROR,
                    i.OUT_OF_LINE,
                    i.Description
                })
                .ToList();
            if (agvSupply.Count == 0) return Json("");
            return new JsonResult(agvSupply, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //
        [HttpGet("GetAgvSupplyDetail/{_fact}/{_dept}/{_block}/{_date}/{_agv}/{_startTime}/{_stt}")]
        public JsonResult GetAgvSupplyDetail(string _fact, string _dept, string _block, string _date, string _agv, string _startTime, string _stt)
        {
            string agvSupplyDetail = null;
            switch (_stt)
            {
                case "normal":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.NORMAL_DETAIL)
                    .First();
                    break;
                case "stop":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.STOP_BY_CARD_DETAIL)
                    .First();
                    break;
                case "safety":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.SAFETY_DETAIL)
                    .First();
                    break;
                case "emergency":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.EMERGENCY_DETAIL)
                    .First();
                    break;
                case "batterylow":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.BATTERY_EMPTY_DETAIL)
                    .First();
                    break;
                case "pole":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.POLE_ERROR_DETAIL)
                    .First();
                    break;
                case "outline":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.OUT_OF_LINE_DETAIL)
                    .First();
                    break;
                case "nocart":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => i.NO_CART_DETAIL)
                    .First();
                    break;
            }

            if (agvSupplyDetail != null)
            {
                //List<string> arr = ((IEnumerable)agvSupplyDetail).Cast<object>()
                //                              .Select(x => x.ToString().Trim().Replace("\r\n", string.Empty))
                //                              .ToList();
                agvSupplyDetail = agvSupplyDetail.TrimEnd(',');
                string[] ret = agvSupplyDetail.Split(',');
                return Json(ret);

            }

            return Json("");

        }

        //
        [HttpGet("GetPartSupply/{_fact}/{_dept}/{_block}/{_date}/{_isDay}/{_isNight}")]
        public JsonResult GetPartSupply(string _fact, string _dept, string _block, string _date, bool _isDay, bool _isNight)
        {
            string day = "";
            if ((_isDay ^ _isNight))
            {
                if (_isDay) day = "day";
                if (_isNight) day = "night";
            }
            List<Part> partSupply = _dBContext.AgvSupplys
                .Where(d => d.Factory == _fact && d.Date == _date && d.Dept == _dept && d.Block == _block && ((day != "") ? d.Shift == day : true))
                .Select(d => new Part { Name = d.Part, Target = d.TotalSupplyTarget, CycleTime = d.CycleTimeTarget })
                .ToList();

            List<PartCount> partCount = partSupply
                .GroupBy(d => d.Name)
                .Select(d => new PartCount
                {
                    Name = d.Key,
                    Count = d.Count()
                })
                .ToList();

            var y = partSupply
                .GroupBy(d => d.Name)
                .Select(g => g.First())
                .Join(partCount, p => p.Name, c => c.Name, (p, c) => new
                {
                    Name = p.Name,
                    Target = p.Target,
                    Cycle = p.CycleTime,
                    Count = c.Count
                })
                .ToList();

            return new JsonResult(y, new JsonSerializerSettings { Formatting = Formatting.Indented });

            //var x = from p in partSupply
            //        join c in partCount on p.Name equals c.Name
            //        select new
            //        {
            //            Name = p.Name,
            //            Target = p.Target,
            //            Cycle = p.CycleTime,
            //            Count = c.Count
            //        };
        }

        // GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
