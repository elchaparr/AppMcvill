using AppMcvill.Data;
using AppMcvill.Models;
using AppMcvill.Popups;
using AppMcvill.Services;
using CommunityToolkit.Maui.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace AppMcvill
{
    public partial class CollectionAPage : ContentPage
    {
        public ObservableCollection<RequisicionItemVM> Requisiciones { get; set; }

        private List<MensajePendiente> MensajesPendientes { get; set; } = new List<MensajePendiente>();
        private readonly IDispatcherTimer timer;
        private bool FiltroActivo = false;
        private readonly WhatsappService _whatsappService = new WhatsappService();
        private List<int> idsNotificados = new List<int>();

        private bool BusquedaActiva => !string.IsNullOrWhiteSpace(txtBuscar.Text);

        public CollectionAPage()
        {
            InitializeComponent();

            Requisiciones = new ObservableCollection<RequisicionItemVM>();
            BindingContext = this;

            CargarDatos(); // Inicializar datos

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += (s, e) =>
            {
                if (BusquedaActiva || FiltroActivo) return;
                ActualizarDatos();
                ProcesarRespuestaAsync();
            };
            timer.Start();
        }

        private void CargarDatos(bool inicializarIds = true)
        {
            using (var db = new DBconection())
            {
                var datos = db.Requisicion_Detail
                    .Where(r => r.Estatus == "Iniciada" &&
                                r.Autorizada != "SI" &&
                                r.Autorizada != "NO")
                    .Select(r => new RequisicionItemVM
                    {
                        Id = r.Id,
                        IdRequisicion = r.IdRequisicion,
                        Linea = r.Linea,
                        Material = r.Material,
                        Descripcion = r.Descripcion,
                        Cantidad = r.Cantidad,
                        UM = r.UM,
                        Tipo = r.Tipo

                    })
                    .OrderByDescending(r => r.IdRequisicion)
                    .ToList();

                Requisiciones.Clear();

                if (inicializarIds)
                    idsNotificados = datos.Select(x => x.Id).ToList();

                foreach (var item in datos)
                    Requisiciones.Add(item);
            }
        }

        
        private void OnBorderTapped(object sender, EventArgs e)
        {
            var border = sender as Border;
            var item = border.BindingContext as RequisicionItemVM;

            if (item != null)
            {
                using (var db = new DBconection())
                {
                    var listaPrueba = db.Requisicion_Prueba
                        .Where(r => r.IdRequi == item.IdRequisicion)
                        .Where(r=> r.Linea==item.Linea)
                        .ToList();


                    string cantidadStr = item.Cantidad.ToString("N2");
                    //string cantidadSug = infoAdicional?.CantidadSugerida.ToString("N2") ?? "0";
                    //string precio = infoAdicional?.Precio.ToString("N2") ?? "0";
                    //string marca = infoAdicional?.Marca ?? "";
                    //string cotizacion = infoAdicional.Cotizacion.ToString();

                    this.ShowPopup(new DetallePopup(
                        item.Material,
                        item.Descripcion,
                        cantidadStr,
                        item.IdRequisicion.ToString(),
                        item.Linea,
                        item.UM,
                        SesionUsuario.UsuarioActual,
                        listaPrueba,
                        () =>
                        {
                            Requisiciones.Clear();
                            CargarDatos();
                        }
                    ));

                }
            }
        }

        private async void ActualizarDatos()
        {
            using (var db = new DBconection())
            {
                var datos = db.Requisicion_Detail
                    .Where(r => r.Estatus == "Iniciada" &&
                                r.Autorizada != "SI" &&
                                r.Autorizada != "NO")
                    .Select(r => new RequisicionItemVM
                    {
                        Id = r.Id,
                        IdRequisicion = r.IdRequisicion,
                        Linea = r.Linea,
                        Material = r.Material,
                        Descripcion = r.Descripcion,
                        Cantidad = r.Cantidad,
                        UM = r.UM,
                        Tipo = r.Tipo
                    })
                    .ToList();

                var nuevos = datos.Where(x => !idsNotificados.Contains(x.Id)).ToList();

                var destinatarios = new List<string> { "528713337397" };

                foreach (var n in nuevos)
                {
                    foreach (var numero in destinatarios)
                    {
                        try
                        {
                            await _whatsappService.EnviarMensajeAsync(
                                numero,
                                n.IdRequisicion.ToString(),
                                n.Linea.ToString(),
                                n.Material
                            );

                            MensajesPendientes.Add(new MensajePendiente
                            {
                                Telefono = numero,
                                Requisicion = n.IdRequisicion,
                                Linea = n.Linea
                            });
                        }
                        catch (Exception ex)
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert(
                                    "Error WhatsApp",
                                    $"No se pudo enviar a {numero}: {ex.Message}",
                                    "OK"
                                );
                            });
                        }
                    }

                    idsNotificados.Add(n.Id); // Marcar como notificado
                }


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var idsBD = datos.Select(x => x.Id).ToList();
                    var idsApp = Requisiciones.Select(x => x.Id).ToList();

                    // Eliminados
                    var eliminados = idsApp.Except(idsBD).ToList();
                    foreach (var id in eliminados)
                    {
                        var item = Requisiciones.FirstOrDefault(x => x.Id == id);
                        if (item != null) Requisiciones.Remove(item);
                        idsNotificados.Remove(id);
                    }

                    // Actualizados
                    foreach (var registroBD in datos)
                    {
                        var registroApp = Requisiciones.FirstOrDefault(x => x.Id == registroBD.Id);
                        if (registroApp != null)
                        {
                            bool cambiado = registroApp.Linea != registroBD.Linea ||
                                            registroApp.Material != registroBD.Material ||
                                            registroApp.Descripcion != registroBD.Descripcion ||
                                            registroApp.Cantidad != registroBD.Cantidad ||
                                            registroApp.UM != registroBD.UM ||
                                            registroApp.Tipo != registroBD.Tipo ||
                                            registroApp.IdRequisicion != registroBD.IdRequisicion;

                            if (cambiado)
                            {
                                registroApp.Linea = registroBD.Linea;
                                registroApp.Material = registroBD.Material;
                                registroApp.Descripcion = registroBD.Descripcion;
                                registroApp.Cantidad = registroBD.Cantidad;
                                registroApp.UM = registroBD.UM;
                                registroApp.Tipo = registroBD.Tipo;
                                registroApp.IdRequisicion = registroBD.IdRequisicion;

                                var idx = Requisiciones.IndexOf(registroApp);
                                Requisiciones.RemoveAt(idx);
                                Requisiciones.Insert(idx, registroApp);
                            }
                        }
                    }

                    // Agregados
                    var agregados = datos.Where(x => !idsApp.Contains(x.Id))
                                         .OrderByDescending(x => x.IdRequisicion)
                                         .ToList();
                    foreach (var item in agregados) Requisiciones.Insert(0, item);
                });
            }
        }

        private void Busqueda(object sender, TextChangedEventArgs e)
        {
            string busqueda = e.NewTextValue?.ToLower();

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                FiltroActivo = false;
                CargarDatos(inicializarIds: false);
                return;
            }

            FiltroActivo = true;

            using (var db = new DBconection())
            {
                var resultados = db.Requisicion_Detail
                    .Where(r =>
                        ((r.Material ?? "").ToLower().Contains(busqueda) ||
                         (r.Descripcion ?? "").ToLower().Contains(busqueda) ||
                         r.IdRequisicion.ToString().Contains(busqueda) ||
                         (r.UM ?? "").ToLower().Contains(busqueda)) &&
                        r.Estatus == "Iniciada"
                    )
                    .Select(r => new RequisicionItemVM
                    {
                        Id = r.Id,
                        IdRequisicion = r.IdRequisicion,
                        Linea = r.Linea,
                        Material = r.Material,
                        Descripcion = r.Descripcion,
                        Cantidad = r.Cantidad,
                        UM = r.UM,
                        Tipo = r.Tipo
                    })
                    .ToList();

                Requisiciones.Clear();
                foreach (var item in resultados)
                    Requisiciones.Add(item);
            }
        }

        private async void Filtros(object sender, EventArgs e)
        {
            var filtro = new FilterSearch();
            filtro.OnSearchCompleted += (listaFiltrada) =>
            {
                FiltroActivo = true;
                Requisiciones.Clear();

                foreach (var item in listaFiltrada)
                {
                    var vm = new RequisicionItemVM
                    {
                        Id = item.Id,
                        IdRequisicion = item.IdRequisicion,
                        Linea = item.Linea,
                        Material = item.Material,
                        Descripcion = item.Descripcion,
                        Cantidad = item.Cantidad,
                        UM = item.UM,
                        Tipo = item.Tipo
                    };
                    Requisiciones.Add(vm);
                }
            };

            await Navigation.PushAsync(filtro);
        }

        private async Task ProcesarRespuestaAsync()
        {
            var respuesta = await _whatsappService.ObtenerRespuestaAsync();
            if (respuesta == null) return;

            string ultimoId = Preferences.Get("ultimo_id_evento", "");
            if (respuesta.id_evento == ultimoId) return;
            Preferences.Set("ultimo_id_evento", respuesta.id_evento);

            string telefono = respuesta.from;
            string accion = respuesta.accion?.ToLower() ?? "";

            var mensajePendiente = MensajesPendientes.FirstOrDefault(m => telefono.EndsWith(m.Telefono));
            if (mensajePendiente == null) return;

            using (var db = new DBconection())
            {
                string telLimpio = telefono.StartsWith("521") ? telefono.Substring(3) :
                                   telefono.StartsWith("52") ? telefono.Substring(2) :
                                   telefono;

                var empleado = db.Empleados.FirstOrDefault(e => e.TelContacto == telLimpio);
                if (empleado == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Empleado no encontrado", "OK");
                    return;
                }

                int idEmpleado = empleado.IdEmpleado;

                var req = db.Requisicion_Detail.FirstOrDefault(r =>
                    r.IdRequisicion == mensajePendiente.Requisicion &&
                    r.Linea == mensajePendiente.Linea
                );

                if (req != null)
                {
                    if (accion.Contains("cancelar"))
                    {
                        req.Autorizada = "NO";
                        req.MotivoRechazo = "";
                        req.FechaAutorizada = DateTime.Now;
                        req.AutorizadoPor = idEmpleado.ToString();
                    }

                    if (accion.Contains("confirmar"))
                    {
                        req.Autorizada = "SI";
                        req.FechaAutorizada = DateTime.Now;
                        req.AutorizadoPor = idEmpleado.ToString();
                    }

                    db.SaveChanges();
                }
            }

            MensajesPendientes.Remove(mensajePendiente);
        }

        private class MensajePendiente
        {
            public string Telefono { get; set; }
            public int Requisicion { get; set; }
            public string Linea { get; set; }
        }
    }
}
