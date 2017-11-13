using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationApi.Models
{
    public class DataAnalyzeContext:DbContext
    {
        public DataAnalyzeContext(DbContextOptions<DataAnalyzeContext> options) : base(options)
        {
        }
        public DbSet<AgvSupply> AgvSupplys { get; set; }
        public DbSet<AgvData> AgvDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgvSupply>().ToTable("AGV_SupplyDetail");
            modelBuilder.Entity<AgvData>().ToTable("AGV_DATA");
        }
    }
}
