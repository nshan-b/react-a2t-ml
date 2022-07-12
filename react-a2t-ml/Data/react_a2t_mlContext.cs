using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using react_a2t_ml.Models;

namespace react_a2t_ml.Data
{
    public class react_a2t_mlContext : DbContext
    {
        //private readonly IConfiguration _config;

        //public react_a2t_mlContext(IConfiguration config) {
        //    _config = config;
        //}
        public react_a2t_mlContext (DbContextOptions<react_a2t_mlContext> options)
            : base(options)
        {
        }

        

        public DbSet<react_a2t_ml.Models.Sentiments>? Sentiments { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer(_config["ConnectionStrings:react_a2t_mlContext"]);
        //}
    }
}
