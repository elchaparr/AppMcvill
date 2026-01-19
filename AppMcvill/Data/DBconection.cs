using AppMcvill.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Data
{
    public class DBconection : DbContext
    {
        public DbSet<UsuarioApp> UsuariosApp { get; set; }
        public DbSet<RequisicionDetail> Requisicion_Detail { get; set; }
        public DbSet<Empleados> Empleados {  get; set; }
        public DbSet<Requisicion> Requisicion_Prueba { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=192.168.1.114\\SQLEXPRESS;Database=MCVILL;User Id=sa;Password=12345;TrustServerCertificate=True");

        }
    }
}
 