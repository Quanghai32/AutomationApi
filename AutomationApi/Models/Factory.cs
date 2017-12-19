using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class Factory
    {
        public int FactoryId { get; set; }
        public string Name { get; set; }
        public List<Dept> Depts { get; set; }
    }
    public class Dept
    {
        public int DeptId { get; set; }
        public string Name { get; set; }
        public List<Block> Blocks { get; set; }
        public Factory Factory { get; set; }
    }
    public class Block
    {
        public int BlockId { get; set; }
        public string Name { get; set; }
        public Dept Dept { get; set; }
    }
}
