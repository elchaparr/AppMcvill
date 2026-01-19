using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Models
{
    public class Detalle : INotifyPropertyChanged
    {
        private string imagenEstado = "check.png";
        private bool estaHabilitado = true;

        public string ImagenEstado
        {
            get => imagenEstado;
            set { imagenEstado = value; OnPropertyChanged(); }
        }

        public bool EstaHabilitado
        {
            get => estaHabilitado;
            set { estaHabilitado = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
