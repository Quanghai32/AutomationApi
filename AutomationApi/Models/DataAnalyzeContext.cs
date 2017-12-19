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
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Dept> Depts { get; set; }
        public DbSet<Factory> Factorys { get; set; }
        public DbSet<Agv_Performance> Agv_Performances { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgvSupply>().ToTable("AGV_SupplyDetail");
            modelBuilder.Entity<AgvData>().ToTable("AGV_DATA");
            modelBuilder.Entity<Block>().ToTable("Block");
            modelBuilder.Entity<Dept>().ToTable("Dept");
            modelBuilder.Entity<Factory>().ToTable("Factory");
            modelBuilder.Entity<Agv_Performance>().ToTable("Agv_Performance");
        }
    }
}
