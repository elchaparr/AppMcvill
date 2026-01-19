//using PassKit;
using AppMcvill.Data;
using AppMcvill.Models;
using AppMcvill.Services;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.EntityFrameworkCore;
namespace AppMcvill.Popups;

public partial class DetallePopup : Popup
{
    public List<Requisicion> Detalles { get; set; }
    private readonly Action _onCloseCallback;
    //private readonly WhatsappService _whatsappService = new WhatsappService();
    public DetallePopup( string material, string descripcion, string cantidad, string id, string linea, string um, string usuario, List<Requisicion> detalles, Action onCloseCallback)
    {
        InitializeComponent();

        MaterialLabel.Text = material;
        DescripcionLabel.Text = descripcion;
        UsuarioLabel.Text = usuario;
        Linealabel.Text = linea;
        UMLabel.Text = um;
        IdLabel.Text = id;
        Detalles = detalles
            .OrderBy(x=>x.Precio)
            .ToList();
        _onCloseCallback = onCloseCallback;

        foreach (var r in Detalles)
        {
            r.CantidadSugeridaTexto =
                r.CantidadSugerida == 0
                    ? string.Empty
                    : r.CantidadSugerida.ToString(CultureInfo.InvariantCulture);
        }

        if (Detalles.Any())
        {
            var precioMinimo = Detalles.First().Precio;

            foreach (var r in Detalles)
            {
                r.EsPrecioMasBajo = r.Precio == precioMinimo;
            }
        }


        btnRechazar.IsVisible = Detalles.Any();
        BindingContext = this;
    }
    
    private void OnClose(object sender, EventArgs e)
    {
        Close();
    }

    private async void Rechazar(object sender, EventArgs e)
    {
        var popup = new MotivoRechazoPopup();
        var comentario = await Application.Current.MainPage.ShowPopupAsync(popup) as string;

        if (comentario == null)
            return;

        using (var dbp = new DBconection())
        {
            foreach (var detalle in Detalles)
            {
                detalle.ImagenEstado = "reject.png";
                detalle.EstaHabilitado = false;

                var detallespru = dbp.Requisicion_Prueba
                    .Where(r => r.IdRequi == detalle.IdRequi)
                    .ToList();

                foreach (var d in detallespru)
                {
                    d.Aceptada = "NO";
                }
            }

            await dbp.SaveChangesAsync();
        }
        int idRequisicion = int.Parse(IdLabel.Text);
        string linea = Linealabel.Text.Trim();

        using (var db = new DBconection())
        {
            var detalle = db.Requisicion_Detail
                .FirstOrDefault(r => r.IdRequisicion == idRequisicion
                                  && (r.Linea ?? "").Trim() == linea);

            if (detalle != null)
            {
                detalle.MotivoRechazo = comentario;
                detalle.Autorizada = "NO";
                detalle.AutorizadoPor = UsuarioLabel.Text;
                detalle.FechaAutorizada = DateTime.Now;

                await db.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert(
                    "Éxito", "Requisición rechazada correctamente", "OK");
            }
        }
        //await Application.Current.MainPage.DisplayAlert(
          //  "Éxito", "Todas las requisiciones fueron rechazadas correctamente", "OK");

        _onCloseCallback?.Invoke();
        Close();
        btnRechazar.IsEnabled = false;
    }


    private async void Autorizar(object sender, EventArgs e)
    {
        var image = sender as Image;
        var requisicionSeleccionada = image?.BindingContext as Requisicion;
        if (requisicionSeleccionada == null) return;
        
        if (string.IsNullOrWhiteSpace(requisicionSeleccionada.CantidadSugeridaTexto))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Validación",
                "Agrega una cantidad sugerida.",
                "OK");
            return;
        }

        if (!decimal.TryParse(
                requisicionSeleccionada.CantidadSugeridaTexto,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal cantidadSugerida))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Validación",
                "La cantidad sugerida no es válida.",
                "OK");
            return;
        }

        if (cantidadSugerida <= 0)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Validación",
                "La cantidad sugerida debe ser mayor a cero.",
                "OK");
            return;
        }

        requisicionSeleccionada.CantidadSugerida = cantidadSugerida;

        var idGrupo = requisicionSeleccionada.IdRequi;
        var cotizacionSeleccionada = (requisicionSeleccionada.Cotizacion ?? "").Trim();

        using (var dbp = new DBconection())
        {
            var registrosGrupo = dbp.Requisicion_Prueba
                .Where(r => r.IdRequi == idGrupo)
                .Where(r=> r.Linea==Linealabel.Text)
                .ToList();

            foreach (var r in registrosGrupo)
                r.Aceptada = "NO";

            var registroAutorizado = registrosGrupo
                .FirstOrDefault(r =>
                    (r.Cotizacion ?? "").Trim()
                    .Equals(cotizacionSeleccionada, StringComparison.OrdinalIgnoreCase));

            if (registroAutorizado != null)
            {
                registroAutorizado.Aceptada = "SI";
                registroAutorizado.CantidadSugerida = cantidadSugerida;
            }

            await dbp.SaveChangesAsync();
        }

        foreach (var d in Detalles.Where(d => d.IdRequi == idGrupo))
        {
            var esAutorizado = (d.Cotizacion ?? "").Trim()
                .Equals(cotizacionSeleccionada, StringComparison.OrdinalIgnoreCase);

            d.ImagenEstado = esAutorizado ? "checkk.png" : "reject.png";
            d.EstaHabilitado = esAutorizado;
            d.EstadoTexto = esAutorizado ? "Autorizado" : "Rechazado";
        }

        btnRechazar.IsEnabled = false;

        int idRequisicion = int.Parse(IdLabel.Text);
        string linea = Linealabel.Text.Trim();

        using (var db = new DBconection())
        {
            var detalle = db.Requisicion_Detail
                .FirstOrDefault(r =>
                    r.IdRequisicion == idRequisicion &&
                    (r.Linea ?? "").Trim() == linea);

            if (detalle != null)
            {
                detalle.Autorizada = "SI";
                detalle.AutorizadoPor = UsuarioLabel.Text;
                detalle.FechaAutorizada = DateTime.Now;

                await db.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert(
                    "Éxito",
                    "Requisición autorizada correctamente",
                    "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se encontró la requisición",
                    "OK");
            }
        }

        _onCloseCallback?.Invoke();
        Close();
    }

}


/*private async void OnCloseClicked(object sender, EventArgs e)
{
    var popup = new MotivoRechazoPopup();
    var comentario = await Application.Current.MainPage.ShowPopupAsync(popup) as string;

    if (comentario == null)
        return;

    int idRequisicion = int.Parse(IdLabel.Text);
    string linea = Linealabel.Text.Trim();

    using (var db = new DBconection())
    {
        var detalle = db.Requisicion_Detail
            .FirstOrDefault(r => r.IdRequisicion == idRequisicion
                              && (r.Linea ?? "").Trim() == linea);

        if (detalle != null)
        {
            detalle.MotivoRechazo = comentario;
            detalle.Autorizada = "NO";
            detalle.AutorizadoPor = UsuarioLabel.Text;
            detalle.FechaAutorizada = DateTime.Now;

            await db.SaveChangesAsync();

            await Application.Current.MainPage.DisplayAlert(
                "Éxito", "Requisición rechazada correctamente", "OK");
        }
    }

    _onCloseCallback?.Invoke();
    Close();
}*/

/*private async void UpdateClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(IdLabel.Text) || string.IsNullOrWhiteSpace(Linealabel.Text))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Id o Línea no válidos", "OK");
                return;
            }

            int idRequisicion = int.Parse(IdLabel.Text);
            string linea = Linealabel.Text.Trim();

            using (var db = new DBconection())
            {
                var detalle = db.Requisicion_Detail
                                .FirstOrDefault(r => r.IdRequisicion == idRequisicion
                                                  && (r.Linea ?? "").Trim() == linea);

                if (detalle != null)
                {
                    detalle.Autorizada = "SI"; // aunque antes fuera null
                    detalle.AutorizadoPor = UsuarioLabel.Text;
                    detalle.FechaAutorizada = DateTime.Now;
                    await db.SaveChangesAsync();

                    await Application.Current.MainPage.DisplayAlert("Éxito", "Requisición autorizada correctamente", "OK");

                    

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se encontró la requisición", "OK");
                }
            }
            _onCloseCallback?.Invoke();
            Close();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Detalle: {ex.Message}", "OK");
        }
    }*/