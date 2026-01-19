namespace AppMcvill;
using AppMcvill.Data;
using AppMcvill.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public partial class FilterSearch : ContentPage
{
    private readonly DBconection _dbContext;
    public event Action<List<RequisicionDetail>> OnSearchCompleted;

    public FilterSearch()
	{
		InitializeComponent();
        _dbContext = new DBconection();
        primero.CheckedChanged += CheckBox_CheckedChanged;
        segundo.CheckedChanged += CheckBox_CheckedChanged;
        foreach (var child in GridUM.Children)
        {
            if (child is CheckBox chk)
            {
                chk.CheckedChanged += CheckBox_CheckedChanged;
            }
        }
        CargarFiltrosGuardados();
        //btnBuscar.IsEnabled = true;

    }

	private async void Cerrar(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("..");
    }

    private async void Buscar(object sender, EventArgs e)
    {
        IQueryable<RequisicionDetail> query = _dbContext.Requisicion_Detail;

        if (primero.IsChecked == true)
        {
            query =query.OrderBy(x=>x.Id).Where(x=>x.Estatus=="Iniciada" && x.Autorizada != "SI" &&
                                x.Autorizada != "NO");
        }
        else if(segundo.IsChecked == true)
        {
            query=query.OrderByDescending(x=>x.Id).Where(x => x.Estatus == "Iniciada" && x.Autorizada != "SI" &&
                                x.Autorizada != "NO");
        }
        else
        {
            query = query.Where(x => x.Estatus == "Iniciada" && x.Autorizada != "SI" && x.Autorizada != "NO").OrderByDescending(x=>x.IdRequisicion);
        }

        List<string> umSeleccionadas = new List<string>();
        for (int i = 0; i < GridUM.Children.Count; i += 2)
        {
            if (GridUM.Children[i] is CheckBox cb && cb.IsChecked)
            {
                if (GridUM.Children[i + 1] is Label lbl)
                    umSeleccionadas.Add(lbl.Text);
            }
        }

        if (umSeleccionadas.Any())
        {
            query = query.Where(x => umSeleccionadas.Contains(x.UM)).Where(x=>x.Estatus=="Iniciada");
        }
        List<string> tiposSeleccionados = new List<string>();

        // Aquí recorres tus layouts de tipos
        if (tercero.IsChecked) tiposSeleccionados.Add("Activo Fijo");
        if (cuarto.IsChecked) tiposSeleccionados.Add("Herramienta");
        if (quinto.IsChecked) tiposSeleccionados.Add("Misceláneo");
        if (sexto.IsChecked) tiposSeleccionados.Add("Producto Fabricado");
        if (septimo.IsChecked) tiposSeleccionados.Add("Suministro");

        if (tiposSeleccionados.Any())
        {
            query = query.Where(x => tiposSeleccionados.Contains(x.Tipo)).Where(x => x.Estatus == "Iniciada");
        }

        if (primero.IsChecked == false || segundo.IsChecked == false)
        {
            query = query.Where(x => x.Estatus == "Iniciada");
        }

        if (!primero.IsChecked && !segundo.IsChecked &&
        !umSeleccionadas.Any() && !tiposSeleccionados.Any())
        {
            query = query.Where(x => x.Estatus == "Iniciada");
        }

        var resultados = await query.ToListAsync();

        OnSearchCompleted?.Invoke(resultados);
        Preferences.Set("filtro_primero", primero.IsChecked);
        Preferences.Set("filtro_segundo", segundo.IsChecked);

        // Guardar UM seleccionadas
        Preferences.Set("filtro_um", string.Join(",", umSeleccionadas));

        // Guardar tipos seleccionados
        Preferences.Set("filtro_tipo_ActivoFijo", tercero.IsChecked);
        Preferences.Set("filtro_tipo_Herramienta", cuarto.IsChecked);
        Preferences.Set("filtro_tipo_Miscelaneo", quinto.IsChecked);
        Preferences.Set("filtro_tipo_ProductoFabricado", sexto.IsChecked);
        Preferences.Set("filtro_tipo_Suministro", septimo.IsChecked);


        await Navigation.PopAsync();

    }

    private void CargarFiltrosGuardados()
    {
        primero.IsChecked = Preferences.Get("filtro_primero", false);
        segundo.IsChecked = Preferences.Get("filtro_segundo", false);

        // Restaurar UM seleccionadas
        string ums = Preferences.Get("filtro_um", "");
        var listaUms = ums.Split(',', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < GridUM.Children.Count; i += 2)
        {
            if (GridUM.Children[i] is CheckBox cb &&
                GridUM.Children[i + 1] is Label lbl)
            {
                cb.IsChecked = listaUms.Contains(lbl.Text);
            }
        }

        // Restaurar tipos seleccionados
        tercero.IsChecked = Preferences.Get("filtro_tipo_ActivoFijo", false);
        cuarto.IsChecked = Preferences.Get("filtro_tipo_Herramienta", false);
        quinto.IsChecked = Preferences.Get("filtro_tipo_Miscelaneo", false);
        sexto.IsChecked = Preferences.Get("filtro_tipo_ProductoFabricado", false);
        septimo.IsChecked = Preferences.Get("filtro_tipo_Suministro", false);
    }

    private void Limpiar(object sender, EventArgs e) 
	{
		primero.IsChecked = false;
		segundo.IsChecked = false;

        foreach (var child in GridUM.Children)
        {
            if (child is CheckBox chk)
            {
                chk.IsChecked = false;
            }
        }

        tercero.IsChecked = false;
        cuarto.IsChecked = false;
        quinto.IsChecked = false;
        sexto.IsChecked = false;
        septimo.IsChecked = false;
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!(sender is CheckBox chk) || !e.Value)
            return;

        if (chk == primero)
        {
            segundo.IsChecked = false;
        }
        else if (chk == segundo)
        {
            primero.IsChecked = false;
        }
    }



}