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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AutomationApi.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class AgvController : Controller
    {
        public struct Department
        {
            public int _id;
            public string _name;
            public List<string> _block;
        }
        public readonly DataAnalyzeContext _dBContext;
        public AgvController(DataAnalyzeContext dbContext)
        {
            _dBContext = dbContext;
        }

        //[HttpGet("Test/{fromDate}/{toDate}/{shift}{fac}/{dept}/{block}")]
        //public JsonResult Test(string fromDate, string toDate, string shift, string fac, string dept, string block)
        //{
        //    AnalyzeData a = new AnalyzeData(_dBContext);
        //    var data = a.GetAgvOperation(fromDate, toDate, shift, fac, dept, block);
        //    return Json(data, new JsonSerializerSettings { Formatting = Formatting.Indented });
        //}

        [HttpGet]
        [Route("GetDeptList")]
        public JsonResult GetDeptList()
        {
            List<Department> DeptList = new List<Department>();
            var query = _dBContext.AgvDatas.Select(d => new { d.Block, d.Dept })
                 .GroupBy(d => d.Block).Select(d => d.First())//.OrderBy(d => d.Dept)
                 .ToList();
            string deptName = "";
            int deptId = 0;

            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].Dept != deptName)
                {
                    DeptList.Add(new Department { _id = deptId++, _name = query[i].Dept, _block = new List<string>() });
                    deptName = query[i].Dept;
                }
            }

            for (int i = 0; i < query.Count; i++)
            {
                for (int deptNum = 0; deptNum < DeptList.Count; deptNum++)
                {
                    if (query[i].Dept == DeptList[deptNum]._name)
                    {
                        DeptList[deptNum]._block.Add(query[i].Block);
                    }
                }
            }

            return Json(DeptList, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet("GetAgvList/{_dept}/{_block}/{_date}")]
        public JsonResult GetAgvList(string _dept, string _block, string _date)
        {
            List<string> listAgv = new List<string>();
            var query = _dBContext.AgvDatas
                .Where(d => d.Dept == _dept && d.Block == _block && d.Date == _date)
                .Select(d => d.Name)
                 .ToList();
            for (int i = 0; i < query.Count; i++)
            {
                listAgv.Add(query[i]);
            }
            return Json(listAgv, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet("GetAgvOperationRate/{_dept}/{_block}/{_fromDate}/{_toDate}")]
        public JsonResult GetAgvOperationRate(string _dept, string _block, string _fromDate, string _toDate)
        {
            var agvRate = _dBContext.AgvDatas.Where(d => d.Date == _fromDate && d.Dept == _dept && d.Block == _block)
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
                    i.Total
                })
                .ToList();
            return Json(agvRate, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //http://localhost:64905/api/agv/GetAgvOperationRate/assy3/d8182/yyyy-MM-dd/yyyy-MM-dd/agv15
        [HttpGet("GetSupplyTime/{_dept}/{_block}/{_fromDate}/{_toDate}/{_agv}")]
        public JsonResult GetSupplyTime(string _dept, string _block, string _fromDate, string _toDate, string _agv)
        {
            var agvSupplyTime = _dBContext.AgvSupplys
                .Where(d => d.Date == _fromDate && d.Dept == _dept && d.Block == _block && d.AgvName == _agv)
                .Select(i => new { i.AgvName, i.SupplyTime, i.StartTime, i.EndTime, i.Route, i.Part })
                .OrderBy(r => r.Route)
                .ToList();

            return Json(agvSupplyTime, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //http://localhost:64905/api/agv/GetAgvSupplyOperation/2017-08-07/assy3/d8182/agv15/09:25:08
        [HttpGet("GetAgvSupplyOperation/{_date}/{_dept}/{_block}/{_agv}/{_startTime}")]
        public JsonResult GetAgvSupplyOperation(string _date, string _dept, string _block, string _agv, string _startTime)
        {
            var agvSupply = _dBContext.AgvSupplys
                .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
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
                    i.OUT_OF_LINE
                })
                .ToList();
            return new JsonResult(agvSupply, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        //http://localhost:64905/api/agv/GetAgvSupplyOperation/2017-08-07/assy3/d8182/agv15/09:25:08/normal
        [HttpGet("GetAgvSupplyDetail/{_date}/{_dept}/{_block}/{_agv}/{_startTime}/{_stt}")]
        public JsonResult GetAgvSupplyDetail(string _date, string _dept, string _block, string _agv, string _startTime, string _stt)
        {
            object agvSupplyDetail = null;
            switch (_stt)
            {
                case "normal":
                    agvSupplyDetail = _dBContext.AgvSupplys
                      .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                      .Select(i => new
                      {
                          i.NORMAL_DETAIL,
                      })
                      .ToList(); break;
                case "stop":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.STOP_BY_CARD_DETAIL,
                    })
                    .ToList();
                    break;
                case "safety":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.SAFETY_DETAIL,
                    })
                    .ToList();
                    break;
                case "emergency":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.EMERGENCY_DETAIL,
                    })
                    .ToList();
                    break;
                case "batterylow":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.BATTERY_EMPTY_DETAIL,
                    })
                    .ToList();
                    break;
                case "pole":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.POLE_ERROR_DETAIL,
                    })
                    .ToList();
                    break;
                case "outline":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.OUT_OF_LINE_DETAIL,
                    })
                    .ToList();
                    break;
                case "nocart":
                    agvSupplyDetail = _dBContext.AgvSupplys
                    .Where(d => d.Date == _date && d.Dept == _dept && d.Block == _block && d.AgvName == _agv && d.StartTime == _startTime)
                    .Select(i => new
                    {
                        i.NO_CART_DETAIL,
                    })
                    .ToList();
                    break;
            }

            if (agvSupplyDetail != null)
            {
                string[] arr = ((IEnumerable)agvSupplyDetail).Cast<object>()
                                             .Select(x => x.ToString())
                                             .ToArray();
                string[] ret = arr[0].Split(',');
                return Json(ret, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            else
            {
                return null;
            }

        }
        //http://localhost:64905/api/agv/GetPartSupplyed/assy3/d8182/2017-08-07/2017-08-07/mid 20
        [HttpGet("GetPartSupplyed/{_dept}/{_block}/{_fromDate}/{_toDate}/{_part}")]
        public JsonResult GetPartSupplyed(string _dept, string _block, string _fromDate, string _toDate, string _part)
        {
            var agvSupplyTime = _dBContext.AgvSupplys
                .Where(d => d.Date == _fromDate && d.Dept == _dept && d.Block == _block && d.Part == _part)
                .Select(i => new { i.AgvName, i.SupplyTime, i.StartTime, i.EndTime, i.Route, i.Part })
                .OrderBy(r => r.AgvName)
                .ToList();

            return Json(agvSupplyTime, new JsonSerializerSettings { Formatting = Formatting.Indented });
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
