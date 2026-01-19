using AppMcvill.Data;
using AppMcvill.Models;
using Microsoft.EntityFrameworkCore;

namespace AppMcvill;

public partial class RejectRequisitionFilter : ContentPage
{
    private readonly DBconection _dbContext;
    public event Action<List<RequisicionDetail>> OnSearchCompleted;
    public RejectRequisitionFilter()
	{
		InitializeComponent();
        _dbContext = new DBconection();
        CargarFiltrosGuardados();
    }

    private async void Cerrar(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void FechaInicio_DateSelected(object sender, DateChangedEventArgs e)
    {
        // La fecha fin no puede ser menor que la de inicio
        FechaFin.MinimumDate = e.NewDate;

        
        if (FechaFin.Date < e.NewDate)
        {
            FechaFin.Date = e.NewDate;
        }
    }

    private async void Buscar(object sender, EventArgs e)
    {
        var query = _dbContext.Requisicion_Detail
            .Where(x => x.Autorizada == "NO");

        // Motivos
        List<string> motivosSeleccionados = new();

        if (tercero?.IsChecked == true)
            motivosSeleccionados.Add("Presupuesto insuficiente");
        if (cuarto?.IsChecked == true)
            motivosSeleccionados.Add("Producto/servicio no disponible");
        if (quinto?.IsChecked == true)
            motivosSeleccionados.Add("Información incompleta o incorrecta");
        if (sexto?.IsChecked == true)
            motivosSeleccionados.Add("Sobre stock de materiales");

        if (motivosSeleccionados.Any())
        {
            query = query.Where(x =>
                x.MotivoRechazo != null &&
                motivosSeleccionados.Contains(x.MotivoRechazo));
        }

        // Fechas SOLO si se modificaron
        DateTime fechaInicio = FechaInicio.Date;
        DateTime fechaFin = FechaFin.Date;

        bool filtroFecha = fechaInicio != DateTime.Today || fechaFin != DateTime.Today;

        if (filtroFecha && fechaInicio <= fechaFin)
        {
            query = query.Where(x =>
                x.FechaAutorizada != null &&
                x.FechaAutorizada.Value.Date >= fechaInicio &&
                x.FechaAutorizada.Value.Date <= fechaFin);
        }

        // Usuarios
        List<string> usuariosSeleccionados = new();

        if (usuario1?.IsChecked == true)
            usuariosSeleccionados.Add("1407");
        if (usuario2?.IsChecked == true)
            usuariosSeleccionados.Add("11");

        if (usuariosSeleccionados.Any())
        {
            query = query.Where(x =>
                x.AutorizadoPor != null &&
                usuariosSeleccionados.Contains(x.AutorizadoPor));
        }
        query = query.OrderByDescending(x => x.FechaAutorizada);
        var resultados = await query.ToListAsync();

        GuardarFiltros();
        OnSearchCompleted?.Invoke(resultados);
        await Navigation.PopAsync();
    }

    private void GuardarFiltros()
    {
        Preferences.Set("filtro_motivo_1", tercero.IsChecked);
        Preferences.Set("filtro_motivo_2", cuarto.IsChecked);
        Preferences.Set("filtro_motivo_3", quinto.IsChecked);
        Preferences.Set("filtro_motivo_4", sexto.IsChecked);

        Preferences.Set("filtro_fecha_inicio", FechaInicio.Date);
        Preferences.Set("filtro_fecha_fin", FechaFin.Date);

        Preferences.Set("filtro_usuario_1", usuario1.IsChecked);
        Preferences.Set("filtro_usuario_2", usuario2.IsChecked);
    }

    private void CargarFiltrosGuardados()
    {
        tercero.IsChecked = Preferences.Get("filtro_motivo_1", false);
        cuarto.IsChecked = Preferences.Get("filtro_motivo_2", false);
        quinto.IsChecked = Preferences.Get("filtro_motivo_3", false);
        sexto.IsChecked = Preferences.Get("filtro_motivo_4", false);

        FechaInicio.Date = Preferences.Get("filtro_fecha_inicio", DateTime.Today);
        FechaFin.Date = Preferences.Get("filtro_fecha_fin", DateTime.Today);

        usuario1.IsChecked = Preferences.Get("filtro_usuario_1", false);
        usuario2.IsChecked = Preferences.Get("filtro_usuario_2", false);
    }


    private void Limpiar(object sender, EventArgs e)
    {
        tercero.IsChecked = false;
        cuarto.IsChecked = false;
        quinto.IsChecked = false;
        sexto.IsChecked = false;

        FechaInicio.Date = DateTime.Today;
        FechaInicio.IsEnabled = true;
        FechaFin.Date = DateTime.Today;
        FechaFin.IsEnabled = true;

        usuario1.IsChecked = false;
        usuario2.IsChecked = false;
    }
}