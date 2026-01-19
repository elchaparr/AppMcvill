using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class Requisicion : INotifyPropertyChanged
    {

        [Key]
        public int Id { get; set; }
        public int IdRequi {  get; set; }
        public string Linea { get; set; }
        public string Cotizacion { get; set; }
        public string Proveedor { get; set; }
        public string Marca { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public string Aceptada { get; set; } = string.Empty;

        private decimal _cantidadSugerida;
        public decimal CantidadSugerida
        {
            get => _cantidadSugerida;
            set
            {
                if (_cantidadSugerida != value)
                {
                    _cantidadSugerida = value;
                    OnPropertyChanged();

                }
            }
        }
        private string _cantidadSugeridaTexto;
        [NotMapped]
        public string CantidadSugeridaTexto
        {
            get => _cantidadSugeridaTexto;
            set
            {
                if (_cantidadSugeridaTexto != value)
                {
                    _cantidadSugeridaTexto = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _espreciobajo;
        [NotMapped]
        public bool EsPrecioMasBajo
        {
            get => _espreciobajo;
            set
            {
                _espreciobajo = value;
                OnPropertyChanged(nameof(EsPrecioMasBajo));
            }
        }

        public string ImagenAceptada
        {
            get
            {
                if (Aceptada == "SI")
                {
                    return "checkk.png";
                }
                    

                if (Aceptada == "NO")
                {
                    return "reject.png";
                }
                    

                return "pendiente.png"; // opcional
            }
        }

        [NotMapped]
        public string EstadoEncabezado =>
    Aceptada == "SI" ? "Autorizado" :
    Aceptada == "NO" ? "Rechazado" :
    "¿Autorizar?";


        private string estadoTexto = "¿Autorizar?";
        [NotMapped]
        public string EstadoTexto
        {
            get => estadoTexto;
            set
            {
                if (estadoTexto != value)
                {
                    estadoTexto = value;
                    OnPropertyChanged();
                }
            }
        }
        private string imagenEstado = "diskette.png";
        [NotMapped]
        public string ImagenEstado
        {
            get => imagenEstado;
            set
            {
                if (imagenEstado != value)
                {
                    imagenEstado = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool estaHabilitado = true;
        [NotMapped]
        public bool EstaHabilitado
        {
            get => estaHabilitado;
            set
            {
                if (estaHabilitado != value)
                {
                    estaHabilitado = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
