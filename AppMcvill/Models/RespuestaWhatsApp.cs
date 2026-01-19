using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class RespuestaWhatsApp
    {
        public string from { get; set; }
        public string accion { get; set; }
        public string textoRecibido { get; set; }
        public string id_evento { get; set; }
    }
}
