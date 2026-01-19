using AppMcvill.Data;
using AppMcvill.Models;
using AppMcvill.Popups;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace AppMcvill;

public partial class CollectionBPage : ContentPage
{
    public ObservableCollection<RequisicionDetail> Requisiciones { get; set; }
    public CollectionBPage()
	{
        InitializeComponent();
        Requisiciones = new ObservableCollection<RequisicionDetail>();
        BindingContext = this;
        CargarDatos();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Requisiciones.Clear();
        CargarDatos();
    }
    private void CargarDatos()
    {
        using (var db = new DBconection())
        {
            var datos = db.Requisicion_Detail
                .Where(r => r.Autorizada == "SI")
                .Select(r => new RequisicionDetail
                {
                    IdRequisicion = r.IdRequisicion,
                    Linea = r.Linea,
                    Material = r.Material,
                    Descripcion = r.Descripcion,
                    Cantidad = r.Cantidad,
                    UM = r.UM,
                    AutorizadoPor = r.AutorizadoPor,
                    FechaAutorizada = r.FechaAutorizada
                }).OrderByDescending(r => r.FechaAutorizada)
                .ToList();

            foreach (var item in datos)
                Requisiciones.Add(item);
        }
    }

    private void OnBorderTappedb(object sender, EventArgs e)
    {
        var border = sender as Border;
        var item = border.BindingContext as RequisicionDetail;

        if (item != null)
        {
            string cantidadStr = item.Cantidad.ToString("N2");
            string idrequi = item.IdRequisicion.ToString();
            string lineast = item.Linea;
            // Abrir popup pasando los valores
            this.ShowPopup(new DetallesAceptadas(item.Material, item.Descripcion, cantidadStr, idrequi, item.AutorizadoPor, lineast, item.UM, () =>
            {
                Requisiciones.Clear(); // limpia
                CargarDatos();         // vuelve a cargar
            }));
        }
    }
}