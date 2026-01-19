using AppMcvill.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class RequisicionDetail
    {
        [Key]
        public int Id { get; set; }
        public int IdRequisicion { get; set; }
        public string Linea { get; set; }
        public string Material { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public string UM { get; set; }
        public string Estatus { get; set; }
        public string Autorizada { get; set; }
        public string Tipo { get; set; }
        public string AutorizadoPor { get; set; }
        public DateTime? FechaAutorizada { get; set; }
        public string MotivoRechazo { get; set; }

        //public bool IsSelected { get; set; } = false;

        public string MaterialFormatted
        {
            get
            {
                if (string.IsNullOrEmpty(Material))
                    return string.Empty;

                int maxCharsPerLine = 20;
                if (Material.Length <= maxCharsPerLine)
                    return Material;

                return Material.Insert(maxCharsPerLine, "\n");
            }
        }

        public string DescriptionFormatted
        {
            get
            {
                if (string.IsNullOrEmpty(Descripcion))
                    return string.Empty;

                int maxCharsPerLine = 26 ;
                if (Descripcion.Length <= maxCharsPerLine)
                    return Descripcion;

                return Descripcion.Insert(maxCharsPerLine, "\n");
            }
        }
    }
}
