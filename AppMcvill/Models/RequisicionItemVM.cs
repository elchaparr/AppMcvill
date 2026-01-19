using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class RequisicionItemVM
    {
        public int Id { get; set; }
        public int IdRequisicion { get; set; }
        public string Linea { get; set; }
        public string Material { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public string UM { get; set; }
        public string Tipo { get; set; }

        public string AutorizadoPor { get; set; }
        public DateTime? FechaAutorizada { get; set; }
        public string MotivoRechazo { get; set; }

        public string MaterialFormatted => string.IsNullOrEmpty(Material) ? "" :
            (Material.Length <= 20 ? Material : Material.Insert(20, "\n"));

        public string DescriptionFormatted => string.IsNullOrEmpty(Descripcion) ? "" :
            (Descripcion.Length <= 26 ? Descripcion : Descripcion.Insert(26, "\n"));
    }

}

