using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class Empleados
    {
        [Key]
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public string TelContacto {  get; set; }
    }
}
