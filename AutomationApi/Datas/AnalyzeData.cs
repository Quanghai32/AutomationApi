using AutomationApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Datas
{
    public class AnalyzeData
    {
        const string PATH_PERFORMANCE = @"D:\AGV Data\PERFORMANCE";
        const string PATH_LOG = @"D:\AGV Data\LOG";
        public AnalyzeData(DataAnalyzeContext dataContext)
        {
            _dataContext = dataContext;
        }
        readonly DataAnalyzeContext _dataContext;
        struct StructAgvOp
        {
            public double Safety;
            public double Stop_By_Card;
            public double Out_Of_Line;
            public double No_Cart;
            public double Normal;
            public double Free;
            public double Pole_Error;
            public double Disconnect;
            public double Shutdown;
            public double Battery_Empty;
            public double Emergency;
        }
        
        public object GetAgvOperation(string fromDate,string toDate,string fac,string shift,string dept,string block)
        {
            List<StructAgvOp> data = _dataContext.AgvDatas
                 .Where(a => a.Date == fromDate && a.Dept == dept && a.Block == block)
                 .Select(a=>new StructAgvOp()
                 {
                     Safety=a.Safety,
                     Stop_By_Card=a.Stop_By_Card,
                     Out_Of_Line=a.Out_Of_Line,
                     No_Cart=a.No_Cart,
                     Free=a.Free,
                     Pole_Error=a.Pole_Error,
                     Disconnect=a.Disconnect,
                     Shutdown=a.Shutdown,
                     Battery_Empty=a.Battery_Empty,
                     Emergency=a.Emergency
                 })
                 .ToList();

            if(data.Count>0)
            {
                return data;
            }
            else
            {
                GetDataFromText(fromDate, toDate, fac, shift, dept, block);
                return new object();
            }
            
        }

        private void GetDataFromText(string fromDate, string toDate, string fac, string shift, string dept, string block)
        {
            
        }

        public void DataTableToJson(DataTable dt, ref string json)
        {
            json = JsonConvert.SerializeObject(dt, Formatting.Indented);
        }
        public void JsonToDataTable(string json, ref DataTable dt)
        {
            dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
        }
    }

}
